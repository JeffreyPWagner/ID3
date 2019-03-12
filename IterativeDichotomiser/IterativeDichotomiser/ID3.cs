using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace IterativeDichotomiser {

    // implements ID3 algorithm
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
        public void readFile(string path) {

            // open file and read line by line, recording data according to lineCount
            StreamReader sr = new StreamReader(path);
            string line;
            while ((line = sr.ReadLine()) != null) {

                // record the number of target classes
                if (lineCount == 0) {
                    Int32.TryParse(line, out numTargets);
                }

                // record the names of the target classes
                else if (lineCount == 1) {
                    targetNames = line.Split(',');
                }

                // record the number of attributes
                else if (lineCount == 2) {
                    Int32.TryParse(line, out numAttributes);
                }

                // record the attributes and their values
                else if (lineCount <= (2 + numAttributes)) {

                    // convert attribute lines to arraylists so they can be manipulated
                    ArrayList lineArray = new ArrayList();
                    lineArray.AddRange(line.Split(','));

                    // name of the attribute on this line
                    string attributeName = lineArray[0].ToString();
                    attributes.Add(attributeName);

                    // record potential values of the attribute on this line
                    numAttributeValues.Add(attributeName, Int32.Parse(lineArray[1].ToString()));
                    string[] possibleValues = new string[Int32.Parse(lineArray[1].ToString())];
                    lineArray.RemoveRange(0, 2);
                    for (int i = 0; i < lineArray.Count; ++i) {
                        possibleValues[i] = lineArray[i].ToString();
                    }
                    attributeValues.Add(attributeName, possibleValues);
                }

                // record the number of examples
                else if (lineCount == (3 + numAttributes)) {
                    Int32.TryParse(line, out numExamples);
                }

                // record the examples
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
            Dictionary<string, Dictionary<string, Dictionary<string, int>>> attributeCounts = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();

            // the total number of examples
            double totalExamples = examples.Count;

            // the entropy of the set of examples, calculated using the target class counts
            double setEntropy = 0;

            // the highest information gain value found for any attribute
            double maxInfoGain = Double.MinValue;

            // the attribute that achieved the highest information gain
            String maxInfoAttribute = "";

            // initialize the attributeCounts dictionary
            foreach (string targ in targetNames) {
                attributeCounts.Add(targ, new Dictionary<string, Dictionary<string, int>>());
                foreach (string attr in attributes) {
                    attributeCounts[targ].Add(attr, new Dictionary<string, int>());
                }
            }

            // count the target classes and attributes in each example
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

            // calculate entropy for the set of examples using the target classes
            foreach (KeyValuePair<String, int> entry in targetCounts) {
                setEntropy -= (entry.Value / totalExamples) * Math.Log((entry.Value / totalExamples), 2.0);
            }

            // calculate the entropy for each attribute value and the information gain for each attribute
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

                // find the attribute that maximizes information gain
                if (infoGain > maxInfoGain) {
                    maxInfoGain = infoGain;
                    maxInfoAttribute = att;
                }
            }
            return maxInfoAttribute;
        }


        // returns a decision tree generated by recursively applying ID3 to a set of input examples
        public TreeNode generateDecisionTree(List<List<String>> examples) {

            // the treenode that will be returned
            TreeNode result = null;

            // tracks if all remaining examples are of the same target class
            bool allSameClass = true;

            // tracks if there are no attributes left to test
            bool noAttributesLeft = true;

            // tracks if method should continue to execute recursively
            bool continueRecursion = true;

            // check to see if a leaf node has been reached
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

            // return a leaf node with the target class label shared by all examples if there is one
            if (allSameClass) {
                result = new TreeNode(examples[0][examples[0].Count - 1], null);
                continueRecursion = false;
            }

            // return a leaf node with the most common target class label if no more attributes to test
            if (noAttributesLeft) {
                result = new TreeNode(mostCommonTarget(examples), null);
                continueRecursion = false;
            }

            // continue if no leaf node generated
            if (continueRecursion) {

                // the attribute that maximizes info gain
                string maxInfoAttribute = findMaxInfoGain(examples);

                // assign the resulting node with the info maximizing attribute
                result = new TreeNode(maxInfoAttribute, examples);

                // split the example set according to the info maximizing attribute
                foreach (string attrVal in attributeValues[maxInfoAttribute]) {
                    List<List<String>> subExamples = new List<List<string>>();
                    foreach (List<String> l in examples) {
                        if (l[attributes.IndexOf(maxInfoAttribute)] == attrVal) {
                            subExamples.Add(l);
                        }
                    }
                    
                    if (subExamples.Count < 1) {
                        continue;
                    }

                    // recursively call method on each split subset of examples
                    result.addChild(generateDecisionTree(subExamples));

                    // assign the new child node with the attribute value that branched to it
                    result.children[result.children.Count - 1].setparAttVal(attrVal);
                }
            }
            return result;
        }


        // finds the most common target class label in a set of examples
        public string mostCommonTarget(List<List<String>> examples) {

            // count of each target class label in the examples
            Dictionary<string, int> targetCounts = new Dictionary<string, int>();

            // tracks the highest label count found
            int maxCount = int.MinValue;

            // tracks the label with the highest count
            string maxTarget = "";

            // count each target class label in the examples
            foreach (List<String> l in examples) {
                if (targetCounts.ContainsKey(l[l.Count - 1])) {
                    targetCounts[l[l.Count - 1]]++;
                }
                else {
                    targetCounts.Add(l[l.Count - 1], 1);
                }
            }

            // compare the counts to find the label with the highest count
            foreach (KeyValuePair<string, int> entry in targetCounts) {
                if (entry.Value > maxCount) {
                    maxCount = entry.Value;
                    maxTarget = entry.Key;
                }
            }
            return maxTarget;
        }


        // recursively prints a decision tree to the console using root node and recursion level inputs
        public void printTree(TreeNode root, int level) {

            
            // stagger data according to tree depth
            for (int i = 0; i < level; i++) {
                Console.Write("\t");
            }
            level++;

            // print node label
            Console.WriteLine(root.attribute);

            // print child labels and branch attribute values
            foreach (TreeNode child in root.children) {
                
                // stagger data according to tree depth
                for (int i = 0; i < level; i++) {
                    Console.Write("\t");
                }
                Console.WriteLine(child.parentAttributeValue);
                printTree(child, level+1);
            }
        }


        // classifies a new example according to an existing decision tree
        public TreeNode classify(TreeNode root, List<string> example) {

            //search for a child node that has the matching attribute value and call method on that node
            for (int i = 0; i < example.Count; i++) {
                foreach (TreeNode child in root.children) {
                    if (child.parentAttributeValue == example[i] && root.attribute == attributes[i]) {
                        return classify(child, example);
                    }
                }
            }
            return root;
        }
    }
}
