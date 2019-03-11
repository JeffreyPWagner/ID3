namespace IterativeDichotomiser {
    class Program {
        static void Main(string[] args) {
            ID3 myID3 = new ID3();
            myID3.readFile();
            myID3.printTree(myID3.generateDecisionTree(myID3.examples),0);
        }
    }
}
