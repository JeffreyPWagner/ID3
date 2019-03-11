using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace IterativeDichotomiser {
    class ID3 {
        int numTargets;
        String[] targetNames;
        public int numAttributes;
        Dictionary<string, int> numAttributeValues = new Dictionary<string, int>();
        Dictionary<string, string[]> attributeValues = new Dictionary<string, string[]>();
        List<string> attributes;
        int numExamples;
        public List<List<String>> examples;
        int lineCount = 0;
        int exampleCount = 0;

        public ID3() {
            attributes = new List<String>();
            examples = new List<List<String>>();
        }

        public void readFile() {
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
        }

        public string findMaxInfoGain(List<List<String>> examples) {
            Dictionary<string, int> targetCounts = new Dictionary<string, int>();
            Dictionary<string, Dictionary<string, Dictionary<string, int>>> attributeCounts = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
            double totalExamples = examples.Count;
            double setEntropy = 0;
            double maxInfoGain = Double.MinValue;
            String maxInfoAttribute = "";
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
            TreeNode result;
            bool allSameClass = true;
            bool noAttributesLeft = true;
            string maxInfoAttribute;
            for (int i = 1; i < examples.Count; i++) {
                List<string> currentEx = examples[i];
                List<string> previousEx = examples[i - 1];
                if (currentEx[currentEx.Count - 1] != previousEx[previousEx.Count - 1]) {
                    allSameClass = false;
                }
                previousEx.RemoveAt(previousEx.Count - 1);
                currentEx.RemoveAt(currentEx.Count - 1);
                if (!currentEx.SequenceEqual(previousEx)) {
                    noAttributesLeft = false;
                }
            }
            if (allSameClass) {
                result = new TreeNode(examples[0][examples[0].Count - 1], null);
            }
            if (noAttributesLeft) {
                result = new TreeNode(mostCommonTarget(examples), null);
            }
            maxInfoAttribute = findMaxInfoGain(examples);
            result = new TreeNode(maxInfoAttribute, examples);
            foreach (string attrVal in attributeValues[maxInfoAttribute]) {
                List<List<String>> subExamples = new List<List<string>>();
                foreach (List<String> l in examples) {
                    if (l[attributes.IndexOf(maxInfoAttribute)] == attrVal) {
                        subExamples.Add(l);
                    }
                }
                result.addChild(new TreeNode(findMaxInfoGain(subExamples), subExamples));
            }
            foreach (TreeNode child in result.children) {
                generateDecisionTree(child.examples);
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
    }
}
