using System;
using System.Collections.Generic;

namespace IterativeDichotomiser {
    class TreeNode {
        public string attribute;
        public List<TreeNode> children;
        public List<List<String>> examples;
        public string parentAttributeValue;

        public TreeNode(string attribute, List<List<String>> examples) {
            this.attribute = attribute;
            this.examples = examples;
            this.parentAttributeValue = null;
            children = new List<TreeNode>();
        }

        public void addChild(TreeNode child) {
            children.Add(child);
        }

        public void setparAttVal(string value) {
            this.parentAttributeValue = value;
        }
    }
}
