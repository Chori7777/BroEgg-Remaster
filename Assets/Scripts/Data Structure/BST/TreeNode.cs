using UnityEngine;

public class TreeNode<T>
{
    public TreeNode<T> left;
    public TreeNode<T> right;
    public T value;

    public TreeNode(T value)
    {
        this.value = value;
    }
}
