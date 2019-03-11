using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace IterativeDichotomiser {

    // class for implementing ID3 algorithm
    class ID3 {

        // the number of target classes in the data
        int numTargets;

        // the names of the target classes
        String[] targetNames;

        // the number of attributes for the examples
        public int numAttributes;

        // the number of potential values for each attribute
        Dictionary<string, int> numAttributeValues = new Dictionary<string, int>();

        // the potential values for each attribute
        Dictionary<string, string[]> attributeValues = new Dictionary<string, string[]>();

        // the attributes for each example
        List<string> attributes;

        // the number of examples in the data set
        int numExamples;

        // a list of all examples
        public List<List<String>> examples;

        // counter for the line of data being read from file
        int lineCount = 0;

        // counter for the number of examples read from file
        int exampleCount = 0;

        // default constructor creates new lists for attributes and examples
        public ID3() {
            attributes = new List<String>();
            examples = new List<List<String>>();
        }


        // reads data from file and stores in class member variables
        public void readFile() {

            // open file and read line by line, recording data according to lineCount
            StreamReader sr = new StreamReader(@"C:\Users\jeffp\OneDrive\Documents\GitHub\CIS_678_Project3\fishing.txt.txt");
            string line;
            while ((line = sr.ReadLine()) != null) {
                if (lineCount == 0) {
                    Int32.TryParse(line, out numTargets);
                }
                else if (lineCount == 1) {
                    targetNames = line.Split(',');
                }
                else if (lineCount == 2) {
                    Int32.TryParse(line, out numAttributes);
                }
                else if (lineCount <= (2 + numAttributes)) {

                    // convert attribute lines to arraylists so they can be manipulated, then extract attribute data
                    ArrayList lineArray = new ArrayList();
                    lineArray.AddRange(line.Split(','));
                    string attributeName = lineArray[0].ToString();
                    attributes.Add(attributeName);
                    numAttributeValues.Add(attributeName, Int32.Parse(lineArray[1].ToString()));
                    string[] possibleValues = new string[Int32.Parse(lineArray[1].ToString())];
                    lineArray.RemoveRange(0, 2);
                    for (int i = 0; i < lineArray.Count; ++i) {
                        possibleValues[i] = lineArray[i].ToString();
                    }
                    attributeValues.Add(attributeName, possibleValues);
                }
                else if (lineCount == (3 + numAttributes)) {
                    Int32.TryParse(line, out numExamples);
                }
                else {
                    examples.Add(new List<String>(line.Split(',')));
                    ++exampleCount;
                }
                ++lineCount;
            }
            sr.Close();
        }


        // returns the attribute in examples that will maximize information gain using entropy
        public string findMaxInfoGain(List<List<String>> examples) {

            // the counts of each target class in the examples
            Dictionary<string, int> targetCounts = new Dictionary<string, int>();

            // the counts of each attribute value in the examples
            Dictionary<string, Dictionary<string, Dictionary<string, int>>> attributeCounts =
                new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();

            // the total number of examples
            double totalExamples = examples.Count;

            // the entropy of the set of examples, calculated using the target class counts
            double setEntropy = 0;

            // the highest information gain value found for any attribute
            double maxInfoGain = Double.MinValue;

            // the attribute that achieved the highest information gain
            String maxInfoAttribute = "";

            //
            foreach (string targ in targetNames) {
                attributeCounts.Add(targ, new Dictionary<string, Dictionary<string, int>>());
                foreach (string attr in attributes) {
                    attributeCounts[targ].Add(attr, new Dictionary<string, int>());
                }
            }
            foreach (List<String> l in examples) {
                string target = l[l.Count - 1];
                int valIndex = 0;
                if (targetCounts.ContainsKey(target)) {
                    targetCounts[target]++;
                }
                else {
                    targetCounts.Add(target, 1);
                }
                l.Remove(target);
                foreach (string attrVal in l) {
                    if (attributeCounts[target][attributes[valIndex]].ContainsKey(attrVal)) {
                        attributeCounts[target][attributes[valIndex]][attrVal]++;
                    }
                    else {
                        attributeCounts[target][attributes[valIndex]].Add(attrVal, 1);
                    }
                    ++valIndex;
                }
                l.Add(target);
            }
            foreach (KeyValuePair<String, int> entry in targetCounts) {
                setEntropy -= (entry.Value / totalExamples) * Math.Log((entry.Value / totalExamples), 2.0);
            }
            foreach (string att in attributes) {
                double infoGain = setEntropy;
                foreach (string attrVal in attributeValues[att]) {
                    double totalAttr = 0;
                    List<int> attValues = new List<int>();
                    foreach (string target in targetNames) {
                        if (attributeCounts[target][att].ContainsKey(attrVal)) {
                            totalAttr += attributeCounts[target][att][attrVal];
                            attValues.Add(attributeCounts[target][att][attrVal]);
                        }
                    }
                    double attEntropy = 0;
                    foreach (int i in attValues) {
                        attEntropy -= (i / totalAttr) * Math.Log((i / totalAttr), 2.0);
                    }
                    infoGain -= (totalAttr / totalExamples) * attEntropy;
                }
                if (infoGain > maxInfoGain) {
                    maxInfoGain = infoGain;
                    maxInfoAttribute = att;
                }
            }
            return maxInfoAttribute;
        }

        public TreeNode generateDecisionTree(List<List<String>> examples) {
            TreeNode result = null;
            bool allSameClass = true;
            bool noAttributesLeft = true;
            bool continueRecursion = true;
            for (int i = 1; i < examples.Count; i++) {
                List<string> currentEx = examples[i];
                List<string> previousEx = examples[i - 1];
                
                
                if (currentEx[currentEx.Count - 1] != previousEx[previousEx.Count - 1]) {
                    allSameClass = false;
                }
                
                string previousTarget = previousEx[previousEx.Count - 1];
                string currentTarget = currentEx[currentEx.Count - 1];
                previousEx.RemoveAt(previousEx.Count - 1);
                currentEx.RemoveAt(currentEx.Count - 1);
                if (!currentEx.SequenceEqual(previousEx)) {
                    noAttributesLeft = false;
                }
                examples[i - 1].Add(previousTarget);
                examples[i].Add(currentTarget);
            }
            if (allSameClass) {
                result = new TreeNode(examples[0][examples[0].Count - 1], null);
                continueRecursion = false;
            }
            if (noAttributesLeft) {
                result = new TreeNode(mostCommonTarget(examples), null);
                continueRecursion = false;
            }
            if (continueRecursion) {
                string maxInfoAttribute = findMaxInfoGain(examples);
                result = new TreeNode(maxInfoAttribute, examples);
                foreach (string attrVal in attributeValues[maxInfoAttribute]) {
                    List<List<String>> subExamples = new List<List<string>>();
                    foreach (List<String> l in examples) {
                        if (l[attributes.IndexOf(maxInfoAttribute)] == attrVal) {
                            subExamples.Add(l);
                        }
                    }
                    result.addChild(generateDecisionTree(subExamples));
                    result.children[result.children.Count - 1].setparAttVal(attrVal);
                }
            }
            return result;
        }

        public string mostCommonTarget(List<List<String>> examples) {
            Dictionary<string, int> targetCounts = new Dictionary<string, int>();
            int maxCount = int.MinValue;
            string maxTarget = "";
            foreach (List<String> l in examples) {
                if (targetCounts.ContainsKey(l[l.Count - 1])) {
                    targetCounts[l[l.Count - 1]]++;
                }
                else {
                    targetCounts.Add(l[l.Count - 1], 1);
                }
            }
            foreach (KeyValuePair<string, int> entry in targetCounts) {
                if (entry.Value > maxCount) {
                    maxCount = entry.Value;
                    maxTarget = entry.Key;
                }
            }
            return maxTarget;
        }

        public void printTree(TreeNode root, int level) {
            for (int i = 0; i < level; i++) {
                Console.Write("\t");
            }
            Console.WriteLine(root.attribute);
            foreach (TreeNode child in root.children) {
                for (int i = 0; i < level; i++) {
                    Console.Write("\t");
                }
                Console.WriteLine(child.parentAttributeValue);
                printTree(child, level+1);
            }
        }
    }
}
