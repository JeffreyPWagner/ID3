using System;
using System.Collections.Generic;

namespace IterativeDichotomiser {

    // implements a node in the ID3 decision tree
    class TreeNode {

        // target class label or attribute label 
        public string attribute;

        // list of child nodes
        public List<TreeNode> children;

        // list of examples still in play at this node
        public List<List<String>> examples;

        // attribute value that branched to this node
        public string parentAttributeValue;

        // default constructor
        public TreeNode(string attribute, List<List<String>> examples) {
            this.attribute = attribute;
            this.examples = examples;
            this.parentAttributeValue = null;
            children = new List<TreeNode>();
        }


        // add a child node to this node
        public void addChild(TreeNode child) {
            children.Add(child);
        }


        // set the attribute value that branched to this node
        public void setparAttVal(string value) {
            this.parentAttributeValue = value;
        }
    }
}
