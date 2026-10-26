using System;
using System.Runtime.InteropServices.WindowsRuntime;
using ED262C;
using TreeEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public class BST<T> where T : IComparable //hace que los genericos implementen la interfaz
{
    int count = 0;
    public int Count { get { return count; } }
    TreeNode<T> root = null;
    public bool IsEmpty { get => Count == 0; }

    public int height { get => GetNodeHeight(root); }
    public int balanceFactor { get => GetNodeBalanceFactor(root); }

    public void Insert(T value)
    {
        root = InsertRecursive(root, value);
    }

    //Recibe un Nodo/posicion, devuelve el nodo que deberia quedar en esa posicion
    protected virtual TreeNode<T> InsertRecursive(TreeNode<T> current, T value)
    {
        //chequeamos si el nodo es null
        //caso base: creamos y devolvemos un nuevo nodo
        if (current == null)
        {
            count++;
            return new TreeNode<T>(value);
        }

        //Usamos una comparacion para saber si el valor es mayor o menor
        int comparison = value.CompareTo(current.value);
        //si es mayor, recursion a la derecha
        if (comparison > 0)
        {
            current.right = InsertRecursive(current.right, value);
        }
        //si es menor, recursion a la izquierda
        if (comparison < 0)
        {
            current.left = InsertRecursive(current.left, value);
        }
        //Devolvemos el mismo nodo que recibimos
        return current;
    }

    public void Remove(T value)
    {
        root = RemoveRecursive(root, value);
    }
    protected virtual TreeNode<T> RemoveRecursive(TreeNode<T> current, T value)
    {
        //Caso base: llegamos al final del arbol y no estaba
        if (current == null)
        {
            return null;
        }

        //Usamos una comparacion para saber si el valor es mayor o menor
        int comparison = value.CompareTo(current.value); // si el valor a eliminar es menor que el valor del current.value -> la comparacion da -1, si el valor a eliminar es mayor al current.value -> 1

        //si es mayor, recursion a la derecha
        if (comparison > 0)
        {
            current.right = RemoveRecursive(current.right, value);
        }
        //si es menor, recursion a la izquierda
        if (comparison < 0)
        {
            current.left = RemoveRecursive(current.left, value);
        }

        //Si son iguales encontramos el valor
        if (comparison == 0)
        {
            count--; //count-- porque removemos

            //Caso 1: no tiene hijos
            if (current.left == null && current.right == null)
            {
                return null;
            }
            else if (current.right == null) //tiene un hijo a la izquierda
            {
                return current.left;
            }
            else if (current.left == null) //tiene un hijo a la derecha
            {
                return current.right;
            }
            else //si tiene ambos hijos
            {
                TreeNode<T> successor = GetMinChild(current.right);
                current.value = successor.value;
                current.right = RemoveRecursive(current.right, successor.value);
                count++; //compensar el count--
            }
        }
        //Devolvemos el mismo nodo que recibimos
        return current;
    }

    //Partiendo de un nodo, itera siempre a la izquierda 
    TreeNode<T> GetMinChild(TreeNode<T> current)
    {
        while (current.left != null) //cuando no haya mas nodos a la izquierda, significa que llegamos al ultimo nodo izquierdo del arbol, o sea, el mas chico
        {
            current = current.left;
        }
        return current;
    }

    public bool Contains(T value)
    {
        return ContainsRecursive(root, value);
    }

    bool ContainsRecursive(TreeNode<T> current, T value)
    {
        if (current == null) return false;

        int comparison = value.CompareTo(current.value);

        if (comparison < 0) return ContainsRecursive(current.left, value);
        if (comparison > 0) return ContainsRecursive(current.right, value);
        if (comparison == 0) return true;

        return false;
    }

    void InOrderDFS(TreeNode<T> current) //Muestra los elemntos ordenados de menor a mayor
    {
        if (current == null) return;

        InOrderDFS(current.left);
        Debug.Log(current.value.ToString());
        InOrderDFS(current.right);
    }
    void PreOrderDFS(TreeNode<T> current) //Muestra los elementos ordenados con la misma jerarquia que el arbol
    {
        if (current == null) return;

        Debug.Log(current.value.ToString());
        PreOrderDFS(current.left);
        PreOrderDFS(current.right);
    }

    void PostOrderDFS(TreeNode<T> current) //Va de abajo hacia arriba, procesa primero los hijos de los nodos antes que los nodos padres
    {
        if (current == null) return;

        PostOrderDFS(current.left);
        PostOrderDFS(current.right);
        Debug.Log(current.value.ToString());
    }

    public void PreOrder() //DFS
    {
        PreOrderDFS(root);
    }
    public void InOrder()//DFS
    {
        InOrderDFS(root);
    }
    public void PostOrder()//DFS
    {
        PostOrderDFS(root);
    }


    protected int GetNodeHeight(TreeNode<T> node)
    {
        if (node == null) return -1;
        return 1 + Math.Max(GetNodeHeight(node.left), GetNodeHeight(node.right));
    }

    protected int GetNodeBalanceFactor(TreeNode<T> node)
    {
        return GetNodeHeight(node.left) - GetNodeHeight(node.right);
    }



    //BFS
    public ISimpleList<T> LevelOrder() //Devuelve una lista ordenada en base a los niveles del arbol
    {
        ISimpleList<T> result = new SimpleArrayList<T>(count); //crea una lista con el tamaño de elementos del arbol
        SimpleLinkedQueue<TreeNode<T>> queue = new(); //crea una linkedQueue
        queue.Enqueue(root); //encola la raiz

        while (queue.Count > 0) //Mientras que haya objetos en la queue
        {
            TreeNode<T> current = queue.Dequeue(); //Descolamos un elemento y lo guardamos en una variable
            result.Add(current.value); //Agregamos a la lista ese valor
            if (current.left != null) queue.Enqueue(current.left); //Encolamos a la queue el objeto de la izquierda del que metimos a la lista
            if (current.right != null) queue.Enqueue(current.right); //Encolamos a la queue el objeto de la derecha del que metimos a la lista
        }   
        return result; //Devolvemos la lista
    }
    // TAREAS MIAS--------------------------------------------------------------------------------------------
    public T FindMin()
    {
        return FindMinRecursive(root);
    }
    public T FindMax()
    {
        return FindMaxRecursive(root);
    }

    public T FindMinRecursive(TreeNode<T> current)
    {
        while (current.left != null)
        {
            current = current.left;
        }
        return current.value;
    }
    public T FindMaxRecursive(TreeNode<T> current)
    {
        while (current.right != null)
        {
            current = current.right;
        }
        return current.value;
    }

    public bool IsValidBST()
    {
        return IsValidBSTRecursive(root, default, default, false, false);
    }

    private bool IsValidBSTRecursive(TreeNode<T> current, T minValue, T maxValue, bool hasMin, bool hasMax)
    {
        // 1. Caso base: si llegamos a un nodo vacío, es válido hasta ahí
        if (current == null) return true;

        // 2. Validar contra el límite mínimo (si está activo)
        if (hasMin && current.value.CompareTo(minValue) <= 0)
            return false;

        // 3. Validar contra el límite máximo (si está activo)
        if (hasMax && current.value.CompareTo(maxValue) >= 0)
            return false;

        // 4. Llamadas recursivas:
        // - Izquierda: el valor máximo permitido ahora pasa a ser el del nodo actual.
        // - Derecha: el valor mínimo permitido ahora pasa a ser el del nodo actual.
        bool leftValid = IsValidBSTRecursive(current.left, minValue, current.value, hasMin, true);
        bool rightValid = IsValidBSTRecursive(current.right, current.value, maxValue, true, hasMax);

        // 5. Retornar true SOLO si ambos subárboles son válidos
        return leftValid && rightValid;
    }
}
