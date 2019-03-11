using System;
using System.IO;

namespace IterativeDichotomiser {
    class Program {
        static void Main(string[] args) {
            ID3 myID3 = new ID3();
            myID3.readFile();
            //Console.WriteLine(myID3.findMaxInfoGain(myID3.examples));
            myID3.generateDecisionTree(myID3.examples);
        }
    }
}
