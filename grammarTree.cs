class Tree<Node>
{
    Node node;
    Tree<Node> UnderTree;
}
class TreeIterator
{
    Tree[] trees;//tree.tree[0].tree[0]
    private int position = -1;
    Tree tree{
        get(){
            return trees[position];
        }
    }
    TreeIterator(Tree baseTree){
        trees.Add(baseTree);
        position = 0;
    }
    void Back(int level = 1){
        for(int i = 0; i < level; i++){
            if(position > 0)
                tree = trees[position - 1];
            else
                throw StackOverflowException();//TODO?
        }
    }
    void Back(Func<Tree, Bool> isBack){
        int i = 0;
        while (isBack(tree))
        {
            Back();
            i++;
        }
        return i;
    }
    void Next(){
        trees.Add(tree.UnderTree);
        position ++;
    }
    void Reset(){
        position = -1;
    }
}
abstract class SymbolNode
{
    enum Bracket
    {
        Angle, Round, Curly, Square
        //<>, (), {}, []
    }
    enum Operator
    {
        
        //+, -, *, /, ...
    }
}
class BracketNode : SymbolNode{
    Bracket BracketType;
    String InnerString;
}
class OperatorNode : SymbolNode{
    Operator OperatorType;
    Tuple<String, String> OperandsString;
}

extern String File;
var tree = new TreeIterator(new TreeNode<SymbolNode>());
for (int i = 0; i < file.Length; i++)
{
    
}