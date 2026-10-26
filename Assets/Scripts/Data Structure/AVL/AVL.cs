using System;
using JetBrains.Annotations;
using UnityEngine;

public class AVL<T> : BST<T> where T : IComparable
{
    protected override TreeNode<T> InsertRecursive(TreeNode<T> current, T value)
    {
        //insertamos normalmente segun BST
        current = base.InsertRecursive(current, value);
        //balanceamos el nodo insertado
        current = RebalanceNode(current);
        //devolvemos el nodo post-balanceado
        return current;
    }

    protected override TreeNode<T> RemoveRecursive(TreeNode<T> current, T value)
    {
        return RebalanceNode(base.RemoveRecursive(current, value));
    }

    TreeNode<T> RebalanceNode(TreeNode<T> node)
    {
        
        int BF = GetNodeBalanceFactor(node);

        //caso Lx
        if(BF > 1)
        {
            if(GetNodeBalanceFactor(node.left) >= 0)
            {
                return RotateRight(node);
            }
            else return RotateLeftRight(node);
        }
        //caso Rx
        else if(BF < -1)
        {
            if (GetNodeBalanceFactor(node.right) <= 0)
            {
                return RotateLeft(node);
            }
            else return RotateRightLeft(node);
        }
        return node;
    }

    TreeNode<T> RotateRight(TreeNode<T> y)
    {
        TreeNode<T> x = y.left;
        TreeNode<T> t2 = x.right;

        x.right = y;
        y.left = t2;

        return x;
    }

    TreeNode<T> RotateLeft(TreeNode<T> x)
    {
        TreeNode<T> y = x.right;
        TreeNode<T> t2 = y.left;

        y.left = x;
        x.right = t2;

        return y;
    }

    TreeNode<T> RotateRightLeft(TreeNode<T> node)
    {
        node.right = RotateRight(node.right);
        return RotateLeft(node);
    }

    TreeNode<T> RotateLeftRight(TreeNode<T> node)
    {
        node.left = RotateLeft(node.left);
        return RotateRight(node);
    }
}
