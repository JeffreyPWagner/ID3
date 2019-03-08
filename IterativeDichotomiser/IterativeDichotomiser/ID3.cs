using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IterativeDichotomiser {
    class ID3 {
        int numTargets;
        String[] targetNames;
        public int numAttributes;
        Dictionary<string, int> numAttributeValues = new Dictionary<string, int>();
        Dictionary<string, string[]> attributeValues = new Dictionary<string, string[]>();
        int numExamples;
        String[][] examples;
        int lineCount = 0;
        int exampleCount = 0;

        public void readFile() {

            StreamReader sr = new StreamReader(@"C:\Users\jeffp\OneDrive\Documents\GitHub\CIS_678_Project3\fishing.txt.txt");
            string line;
            while ((line = sr.ReadLine()) != null) {
                Console.WriteLine(line);
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
                    string[] possibleValues = new string[Int32.Parse(lineArray[1].ToString())];
                    lineArray.RemoveRange(0, 2);
                    for (int i = 0; i < lineArray.Count; ++i) {
                        possibleValues[i] = lineArray[i].ToString();
                    }
                    attributeValues.Add(attributeName, possibleValues);
                }
                else if (lineCount == (3 + numAttributes)) {
                    Int32.TryParse(line, out numExamples);
                    examples = new string[numExamples][];
                }
                else {
                    examples[exampleCount] = line.Split(',');
                    ++exampleCount;
                }
                ++lineCount;
            }
        }
    }
}
