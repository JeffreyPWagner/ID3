using System;
using System.Collections.Generic;

namespace IterativeDichotomiser {

    // executes ID3 on selected data sets
    class Program {
        static void Main(string[] args) {

            // create an ID3 for the fishing data
            ID3 fishingID3 = new ID3();

            //create an ID3 for the contact lense data
            ID3 contactID3 = new ID3();

            // create an ID3 for the car training data
            ID3 trainingID3 = new ID3();

            //create an ID3 for the car test data, used only to extract the examples
            ID3 testID3 = new ID3();

            // read the files
            fishingID3.readFile(@"C:\Users\jeffp\OneDrive\Documents\GitHub\CIS_678_Project3\fishing.txt");
            contactID3.readFile(@"C:\Users\jeffp\OneDrive\Documents\GitHub\CIS_678_Project3\contact-lenses.txt");
            trainingID3.readFile(@"C:\Users\jeffp\OneDrive\Documents\GitHub\CIS_678_Project3\car_training.txt");
            testID3.readFile(@"C:\Users\jeffp\OneDrive\Documents\GitHub\CIS_678_Project3\car_test.txt");

            //generate and print fishing and contact decsion trees
            TreeNode contactDecisionTree = contactID3.generateDecisionTree(contactID3.examples);
            Console.WriteLine("Contact Decision Tree:");
            contactID3.printTree(contactDecisionTree, 0);
            Console.WriteLine();
            TreeNode fishingDecisionTree = fishingID3.generateDecisionTree(fishingID3.examples);
            Console.WriteLine("Fishing Decision Tree:");
            fishingID3.printTree(fishingDecisionTree, 0);
            Console.WriteLine();

            //create HW2 examples and classify according to attributes available in the fishing data
            List<string> exampleA = new List<string>();
            exampleA.Add("Strong");
            exampleA.Add("Warm");
            exampleA.Add("Cold");
            exampleA.Add("Sunny");
            List<string> exampleB = new List<string>();
            exampleB.Add("Weak");
            exampleB.Add("Cold");
            exampleB.Add("Warm");
            exampleB.Add("Rainy");
            Console.WriteLine("Homework 2 examples: ");
            Console.Write("Data A: ");
            Console.WriteLine(fishingID3.classify(fishingDecisionTree, exampleA).attribute);
            Console.Write("Data B: ");
            Console.WriteLine(fishingID3.classify(fishingDecisionTree, exampleB).attribute);
            Console.WriteLine();

            // generate car training decision tree
            TreeNode carDecisionTree = trainingID3.generateDecisionTree(trainingID3.examples);

            // initialize accuracy metrics
            double correctPercent;
            double correctCount = 0;

            // classify the test examples and increment correctCount if they are correct
            foreach (List<string> example in testID3.examples) {
                string answer = example[example.Count - 1];
                example.RemoveAt(example.Count - 1);
                if (answer == (trainingID3.classify(carDecisionTree, example).attribute)) {
                    correctCount++;
                }
            }

            // calculate and print the percentage of examples correctly classified
            correctPercent = correctCount / (double)testID3.examples.Count * 100;
            Console.Write("Percentage of car test examples correctly classified: ");
            Console.WriteLine(correctPercent);
            Console.WriteLine();

            // print the car decision tree
            Console.WriteLine("Car Decision Tree: ");
            trainingID3.printTree(carDecisionTree, 0);
        }
    }
}
