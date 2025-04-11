using ParallelTree_Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Symbolics;
using Expression = MathNet.Symbolics.SymbolicExpression;

namespace AlgebraicLaws
{
    public static class LawOptimization
    {
        public static string[] AllForms(string expression)
        {
            expression = Simplify(expression);

            return null;
        }

        public static string Simplify(Expression expression)
        {
            return Infix.Format(Algebraic.Expand(expression.Expression));
        }   
        /*public static ParallelTree_Builder.TreeNode OptimizeDistributive(ParallelTree_Builder.TreeNode Node)
        {
            TreeMultiNode MultiNode = Node.ToMultiNodeTree();
            return Node;
        }

        public static ParallelTree_Builder.TreeNode OptimizeAssociative(ParallelTree_Builder.TreeNode Node)
        {
            
            TreeMultiNode MultiNode = Node.ToMultiNodeTree();
            return Node;
        }*/
    }
}
