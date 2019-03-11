using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace IterativeDichotomiser {
    class TreeNode {
        string attribute;
        public List<TreeNode> children;
        public List<List<String>> examples;

        public TreeNode(string attribute, List<List<String>> examples) {
            this.attribute = attribute;
            this.examples = examples;
            children = new List<TreeNode>();
        }

        public void addChild(TreeNode child) {
            children.Add(child);
        }
    }
}
