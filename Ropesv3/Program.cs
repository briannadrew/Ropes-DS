/* Name: Rope Data Structure
 * Author: Brianna Drew
 * Date Created: February 10th, 2020
 * Last Modified: March 8th, 2020
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ropesv3
{
    public static class MyGlobals
    {
        public const int MAX_LEAF = 10;     // maximum number of characters to be stored per leaf node
        public static bool EXIT, ERROR, FIRST = true, SPLIT_RIGHT;
        public static int SUB_COUNT = 0, SUB_INDEX = 0, SPLIT_INDEX = 0, SPLIT_COUNT = 0;
    }

    // linked list
    public class Node<T>
    {
        public Node<T> Left { get; set; }
        public Node<T> Right { get; set; }
        public Node<T> Parent { get; set; }
        public char[] Str { get; set; }
        public int LCount { get; set; }

        public Node()
        {
            Left = null;
            Right = null;
            Parent = null;
            LCount = 0;
        }

    }

    public class Ropes<T>
    {
        public Node<T> Root { get; set; }

        // Rope
        // Creates a new rope from passed string
        public void Rope(char[] S)
        {
            List<Node<T>> temp_nodes = new List<Node<T>>();
            int index_count = 0;
            int right_i = S.Length - 1;
            double length = right_i + 1;
            int leaf_nodes = Convert.ToInt32(Math.Ceiling(length / MyGlobals.MAX_LEAF));

            for (int count = 0; count < leaf_nodes; count++)        // for each leaf node...
            {
                Node<T> temp = new Node<T>      // create and initialize the new node
                {
                    Left = null,
                    Right = null,
                    Parent = null,
                    Str = new char[MyGlobals.MAX_LEAF]
                };
                for (int thisindex_count = 0; thisindex_count < MyGlobals.MAX_LEAF && index_count <= right_i; index_count++, thisindex_count++)     // add the appropriate characters from the string to the node's character array
                {
                    temp.Str[thisindex_count] = S[index_count];
                    temp.LCount++;
                }
                temp_nodes.Add(temp);       // add the node to the temporary node list
            }

            index_count = 0;
            int nodes = Convert.ToInt32(Math.Ceiling((double)leaf_nodes / 2));
            int prev_nodes = leaf_nodes;
            int levelnode_count = 0;
            while (nodes >= 2)      // each run through while loop represents a different level of the tree (excluding the leaf node and root node levels)
            {
                for (int counter = 0; counter < nodes; counter++)       // for each node at current level
                {
                    Node<T> temp = new Node<T>      // create and initialize the new node
                    {
                        Left = null,
                        Right = null,
                        Parent = null
                    };
                    for (int counter2 = 1; counter2 <= 2 && levelnode_count < prev_nodes; counter2++, index_count++, levelnode_count++)   // for each child of the node
                    {
                        if (counter2 == 1)      // left child
                        {
                            temp.Left = temp_nodes[index_count];        // link left child to parent and vice versa
                            temp.LCount = temp.Left.LCount;     // calculate the weight
                            if (temp.Left.Right != null)        // if we need to consider the right subtree in weight calculation...
                            {
                                temp.LCount += RightCount(temp.Left.Right, 0);      // re-calculate the weight
                            }
                            temp_nodes[index_count].Parent = temp;
                        }
                        if (counter2 == 2)      // right child
                        {
                            temp.Right = temp_nodes[index_count];       // link right child to parent and vice versa
                            temp_nodes[index_count].Parent = temp;
                        }
                    }
                    temp_nodes.Add(temp);       // add the node to the temporary node list
                }
                levelnode_count = 0;
                prev_nodes = nodes;
                nodes = Convert.ToInt32(Math.Ceiling((double)nodes / 2));
            }

            Node<T> root = new Node<T>      // create and initialize the root node of the rope
            {
                Parent = null,
            };

            if (temp_nodes.Count == 1)      // if there is only one other node (left node)...
            {
                root.Left = temp_nodes[0];      // create links between root and left child
                root.Right = null;
                root.LCount = root.Left.LCount;
                temp_nodes[0].Parent = root;
            }
            else        // if there is more than one other node...
            {
                root.Left = temp_nodes[temp_nodes.Count - 2];       // create the links between root and children
                root.Right = temp_nodes[temp_nodes.Count - 1];
                root.LCount = root.Left.LCount;     // calculate the weight
                if (root.Left.Right != null)        // if we need to consider the right subtree in weight calculation...
                {
                    root.LCount += RightCount(root.Left.Right, 0);      // re-calculate the weight
                }
                temp_nodes[temp_nodes.Count - 2].Parent = root;
                temp_nodes[temp_nodes.Count - 1].Parent = root;
            }

            Root = root;        // set root property of rope
            temp_nodes.Add(root);       // add the root to the temporary rope node list
        }

        // RightCount
        // Counts the characters in the right side of an existing rope structure from a starting node
        private int RightCount(Node<T> curr, int count)
        {
            if (curr != null)   // if the current node is not null...
            {
                count += curr.LCount;       // add current weight to total weight
                return RightCount(curr.Right, count);      // go to the right
            }
            return count;
        }

        // GetTotal
        // Counts the total number of characters in a given rope
        public int GetTotal()
        {
            int count = Root.LCount;        // set count equal to the count of the left tree
            count += RightCount(Root, 0);       // add the count of the right tree to the total count
            return count;
        }

        // Insert
        // Inserts a substring into an existing rope structure at index i
        public Ropes<T> Insert(char[] S, int i)
        {
            int length = 0;
            foreach (char element in S)
            {
                if (element != default(char))
                {
                    length++;       // determine length of S
                }
            }
            Ropes<T> newrope = new Ropes<T>();     // create new rope
            newrope.Rope(S);        // Create a new rope with the string to be inserted
            Ropes<T> splitrope = Split(i);      // Split current rope at index i
            newrope = newrope.Concatenate(splitrope);       // concatenate the new rope of S the the split rope (middle & last)
            return Concatenate(newrope);        // concatenate the rope to the previously concatenated rope (first & middle)
        }

        // Delete
        // Deletes a substring from an existing rope structure from indexes i - j
        public Ropes<T> Delete(int i, int j)
        {
            Ropes<T> splitrope = Split(j);      // split the rope at second index
            _ = Split(i);   // split the rope at first index and discard (what we wanted to delete)
            return Concatenate(splitrope);      // concatenate the "bookend" ropes back together
        }

        // Substring
        // Returns a substring in an existing rope structure from indexes i - j
        public string Substring(Node<T> curr, int i, int j, int index_count)
        {
            char[] char_sub = new char[(j - i) + 1];
            string substring = null;
            if (curr == null)       // if the current node is null, return null
            {
                return null;
            }
            if (MyGlobals.FIRST)        // if we are at the beginning (root)
            {
                curr = Root;        // set the current node to the root node
                MyGlobals.FIRST = false;
                if (i < curr.LCount && j >= curr.LCount)        // if i and j are on either subtrees of the root...
                {
                    return Substring(curr, i, j, char_sub);     // start traversal from the root
                }
                else if (i >= curr.LCount)      // if both i and j are to the right of the root...
                {
                    index_count = curr.LCount;      // increase current index
                    MyGlobals.SUB_INDEX = curr.LCount;
                    return Substring(curr.Right, i, j, index_count);        // go to the right
                }
                else { return Substring(curr.Left, i, j, index_count); }        // if both i and j are to the left of the root, go to the left
            }

            if (((j >= (index_count + curr.LCount)) && (i < (index_count + curr.LCount))) || (curr.Left == null && curr.Right == null))        // if j is in the right subtree and i is in the left subtree of the current node
            {
                return Substring(curr, i, j, char_sub);      // call other substring function to traverse tree starting at this node
            }
            else if (i >= (index_count + curr.LCount))      // if the i index is in the right subtree
            {
                index_count += curr.LCount;     // increase the current index
                MyGlobals.SUB_INDEX += curr.LCount;
                return Substring(curr.Right, i, j, index_count);       // go to the right subtree of current node
            }
            else if (j < (index_count + curr.LCount))       // if the j index is in the left subtree
            {
                return Substring(curr.Left, i, j, index_count);        // go to the left subtree of current node
            }
            return substring;
        }

        public string Substring(Node<T> curr, int i, int j, char[] substring)
        {
            if (curr == null)       // if the current node is null, return null
            {
                return null;
            }
            // IN-ORDER TRAVSERSAL
            Substring(curr.Left, i, j, substring);      // go to the left subtree
            if (curr.Str != null)
            {
                foreach (char element in curr.Str)      // for each character in current substring
                {
                    if (element != default(char))
                    {
                        if (MyGlobals.SUB_INDEX >= i && MyGlobals.SUB_INDEX <= j)       // if the current character is within the desired index range...
                        {
                            substring[MyGlobals.SUB_COUNT] = element;       // add to the substring array
                            MyGlobals.SUB_COUNT++;
                        }
                        MyGlobals.SUB_INDEX++;
                    }
                }
            }
            if (MyGlobals.SUB_INDEX > j)        // if we have found all the characters in the substring...
            {
                string return_string = new string(substring);
                return return_string;       // return the substring
            }
            Substring(curr.Right, i, j, substring);     // go to the right subtree
            string return_string_1 = new string(substring);
            return return_string_1;
        }

        // CharAt
        // Returns the character in an existing rope structure at index i
        public char CharAt(Node<T> curr, int i, int index_count)
        {
            if (curr == null)       // if the current node is null, return null
            {
                return default;
            }
            if ((i >= (index_count + curr.LCount)) && curr.Right != null)       // if the index is to the right of the current node...
            {
                index_count += curr.LCount;
                return CharAt(curr.Right, i, index_count);      // go to the right
            }
            else if (curr.Left != null)     // if the index is to the left of the current node...
            {
                return CharAt(curr.Left, i, index_count);       // go to the left
            }
            for (int count = 0; count < 10; count++)        // if we are at the node that contains the desired character, go through each character
            {
                if (curr.Str[count] != default)
                {
                    if (index_count == i)       // if we are at the desired index...
                    {
                        return curr.Str[count];     // return the character at desired index
                    }
                    index_count++;
                }
            }
            return default;
        }

        // GetNodeAt
        // Returns the node in an existing rope structure at index i
        public Node<T> GetNodeAt(Node<T> curr, int i, int index_count)
        {
            if (curr == null)       // if the current node is null, return null
            {
                return null;
            }
            if ((i >= (index_count + curr.LCount)) && curr.Right != null)       //  if the index is to the right of the current node...
            {
                index_count += curr.LCount;
                MyGlobals.SPLIT_INDEX = index_count;
                return GetNodeAt(curr.Right, i, index_count);       // go to the right
            }
            else if (curr.Left != null)     // if the index is to the left of the current node
            {
                return GetNodeAt(curr.Left, i, index_count);        // go to the left
            }

            if (curr == curr.Parent.Right)      // if the leaf node we ended up at is the right child of it's parent...
            {
                MyGlobals.SPLIT_RIGHT = true;
            }
            else        // if the leaf node we ended up at is the left child of it's parent...
            {
                MyGlobals.SPLIT_RIGHT = false;
            }
            return curr;        // if we are at the node containing the index, return the node
        }

        // Split
        // Splits an existing rope structure into two ropes at index i
        public Ropes<T> Split(int i)
        {
            Node<T> curr = GetNodeAt(Root, i, 0);   // determine node that contains index
            Node<T> leaf1 = new Node<T>();
            Node<T> leaf2 = new Node<T>();
            List<Node<T>> split_list = new List<Node<T>>();
            int char_count = 0;
            char[] node1_ch = new char[MyGlobals.MAX_LEAF];
            char[] node2_ch = new char[MyGlobals.MAX_LEAF];
            foreach(char element in curr.Str)
            {
                if (element != default(char))       // go through each character in node's substring until index is reached
                {
                    if (MyGlobals.SPLIT_INDEX <= i)      // if index not reached yet...
                    {
                        node1_ch[char_count] = element;     // store character in left node
                        MyGlobals.SPLIT_INDEX++;
                        char_count++;
                    }
                    else        // if index is reached...
                    {
                        goto MidSplit;
                    }
                }
            }

            // if index is the last character in the substring...
            curr = GetNodeAt(Root, i + 1, 0);       // get the next leaf node AFTER current node (to the right)
            return Split(curr, split_list);     

        // if index is in the middle of node's char array (not at the end)
        MidSplit:
            int count = 0;

            for (char_count = char_count; char_count < MyGlobals.MAX_LEAF; char_count++)
            {
                node2_ch[count] = curr.Str[char_count];     // copy remaining characters into the second new leaf node
                count++;
            }
            leaf1.Str = node1_ch;
            leaf2.Str = node2_ch;

            foreach (char element in leaf1.Str)
            {
                if (element != default(char))
                {
                    leaf1.LCount++;     // determine weight of leaf node 1
                }
            }
            foreach (char element in leaf2.Str)
            {
                if (element != default(char))
                {
                    leaf2.LCount++;     // determine weight of leaf node 2
                }
            }
            curr.Str = default;     // reset current node and link new leaf nodes as children of current node
            curr.LCount = leaf1.LCount;
            curr.Left = leaf1;
            curr.Right = leaf2;
            leaf1.Parent = curr;
            leaf2.Parent = curr;
            return Split(leaf2, split_list);
        }

        public Ropes<T> Split(Node<T> curr, List<Node<T>> split_list)
        {
            MyGlobals.SPLIT_COUNT++;
            if (curr != null && curr.Right != null)     // if current node is not null and there is a right link...
            {
                if(MyGlobals.SPLIT_COUNT == 2 && !MyGlobals.SPLIT_RIGHT)
                {
                    return Split(curr.Parent, split_list);      // recursively call this function, going up the tree and cutting right links
                }
                if (MyGlobals.SPLIT_COUNT == 3 && curr.Right.Left != null && MyGlobals.SPLIT_RIGHT)      // If the splitting index was in the middle of a char array, we cannot add the original node as that will include the first part of the array we split from
                {
                    return Split(curr.Parent, split_list);      // recursively call this function, going up the tree and cutting right links
                }
                curr.Right.Parent = null;
                split_list.Add(curr.Right);     // cut right node from current tree and add to list
                curr.Right = null;
            }
            else if (curr == null)      // if current node is null (we are done our upwards traversal)...
            {
                Node<T> root = new Node<T>();
                split_list.Add(root);       // add the root to the list of nodes of the new rope
                Ropes<T> split_rope = new Ropes<T>();
                Ropes<T> prev = new Ropes<T>();       // temporary node to represent the previous node in upcoming for loop
                int length = split_list.Count;
                for (int count = 0; count < length; count++)        // for each node in the temporary list...
                {
                    if (count > 0)      // if not the first node of the list...
                    {
                        split_rope.Root = split_list[count];        // make the current node into it's own temporary rope
                        split_rope.Root.LCount = split_list[count].LCount;
                        if (split_list[count].Right != null)        // if we need to consider the right subtree in weight calculation...
                        {
                            split_rope.Root.LCount += RightCount(split_list[count].Right, 0);      // re-calculate the weight
                        }
                        prev = prev.Concatenate(split_rope);        // add the current node to the previous ones by concatenation
                    }
                    else        // if we are at the first node in the list...
                    {
                        prev.Root = split_list[count];      // make the first node in the list the previous tree to start
                        prev.Root.LCount = split_list[count].LCount;
                    }
                }
                MyGlobals.SPLIT_COUNT = 0;
                MyGlobals.SPLIT_INDEX = 0;
                return prev;        // return the new rope
            }
            return Split(curr.Parent, split_list);      // recursively call this function, going up the tree and cutting right links
        }

        // Concatenate
        // Concatenates two existing rope structures into one rope
        public Ropes<T> Concatenate(Ropes<T> R2)
        {
            Ropes<T> R3 = new Ropes<T>();       // create new rope
            Node<T> temp = new Node<T>      // create new node to be root of new rope
            {
                Parent = null,
                Left = Root,
                Right = R2.Root,
                LCount = Root.LCount,
                Str = null
            };
            Root.Parent = temp;     // link two existing ropes to new root
            R2.Root.Parent = temp;
            R3.Root = temp;
            R3.Root.LCount = Root.LCount + R2.Root.LCount;
            return R3;
        }

        // Print
        // Prints the contents of an existing rope structure
        public void Print(Node<T> curr)
        {
            if (MyGlobals.FIRST == true)
            {
                curr = Root;        // if we are at beginning of tree, current node is the root
                MyGlobals.FIRST = false;
            }

            if (curr == null)       // if current node is null, return null
            {
                return;
            }
            if (curr.Left == null && curr.Right == null && curr.Str != null)        // if current node is a lead node...
            {
                foreach (char element in curr.Str)
                {
                    if (element != default(char))
                    {
                        Console.Write(element);     // print substring
                    }
                }
            }
            Print(curr.Left);       // print left subtree
            Print(curr.Right);      // print right subtree

        }
    }

    class Program
    {
        // Main
        // Main program, driver code
        static void Main(string[] args)
        {
            Program program = new Program();
            // Ropes
            Ropes<char[]> R1 = new Ropes<char[]>();
            Ropes<char[]> R2 = new Ropes<char[]>();
            Ropes<char[]> R3 = new Ropes<char[]>();
            int int_choice, int_rope_choice, indexi_choice, indexj_choice, length;
            string choice, input_string, re_str;
            char re_ch;
            char[] string_arr1, string_arr2, string_arri;
            bool conversion;
            MyGlobals.EXIT = false;

            while (!MyGlobals.EXIT)
            {
                // visual menu
                MyGlobals.ERROR = true;
                Console.WriteLine("******************************************************");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("*                       ROPES                        *");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("******************************************************");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("*    1 = CREATE NEW ROPE                             *");
                Console.WriteLine("*    2 = INSERT SUBSTRING                            *");
                Console.WriteLine("*    3 = DELETE SUBSTRING                            *");
                Console.WriteLine("*    4 = FIND CHARACTER                              *");
                Console.WriteLine("*    5 = FIND SUBSTRING                              *");
                Console.WriteLine("*    6 = SPLIT ROPE                                  *");
                Console.WriteLine("*    7 = CONCATENATE ROPES                           *");
                Console.WriteLine("*    8 = PRINT ROPE                                  *");
                Console.WriteLine("*    9 = EXIT PROGRAM                                *");
                Console.WriteLine("*                                                    *");
                Console.WriteLine("******************************************************\n");

                do
                {
                    choice = Console.ReadLine();        // get user input
                    conversion = Int32.TryParse(choice, out int_choice);        // ensure input is an integer
                    if (!conversion)        // if input is not an integer...
                    {
                        Console.WriteLine("ERROR: Invalid option. Please try again.\n");         // print error message
                    }
                    else        // if input is an integer...
                    {
                        MyGlobals.ERROR = false;        // not an error
                        break;
                    }
                    Console.WriteLine();
                } while (MyGlobals.ERROR);

                MyGlobals.ERROR = true;
                Console.WriteLine();

                switch (int_choice)
                {
                    case 1:     // Create a new rope (up to 4 at a time)
                        int_rope_choice = program.RopeChoice();     // get rope
                        Console.WriteLine("What string would you like to create a rope with?");
                        input_string = Console.ReadLine();      // get string from user
                        switch (int_rope_choice)
                        {
                            case 1:
                                string_arr1 = input_string.ToCharArray();
                                R1.Rope(string_arr1);       // create R1 rope
                                Console.WriteLine("Rope 1 created successfully.\n");
                                break;
                            case 2:
                                string_arr2 = input_string.ToCharArray();
                                R2.Rope(string_arr2);       // create R2 rope
                                Console.WriteLine("Rope 2 created successfully.\n");
                                break;
                            default:
                                Console.WriteLine("ERROR: Rope does not exist. Returning to main menu...\n");       // print error message
                                break;
                        }
                        break;

                    case 2:     // Insert a substring into an existing rope
                        int_rope_choice = program.RopeChoice();     // get rope
                        Console.WriteLine("What string would you like to insert?");
                        input_string = Console.ReadLine();      // get substring from user
                        string_arri = input_string.ToCharArray();       
                        indexi_choice = program.IntChoice();        // get index
                        switch (int_rope_choice)
                        {
                            case 1:
                                if (R1.Root == null)
                                {
                                    Console.WriteLine("ERROR: Rope is empty. Returning to main menu...\n");
                                    break;
                                }
                                length = R1.GetTotal();
                                if (indexi_choice >= 0 || indexi_choice < length)
                                {
                                    R1 = R1.Insert(string_arri, indexi_choice);      // insert substring into R1
                                    Console.WriteLine("Rope 1 after insertion: ");
                                    R1.Print(R1.Root);
                                }
                                else { Console.WriteLine("ERROR: Index out of range. Returning to main menu...\n"); }
                                break;
                            case 2:
                                if (R2.Root == null)
                                {
                                    Console.WriteLine("ERROR: Rope is empty. Returning to main menu...\n");
                                    break;
                                }
                                length = R2.GetTotal();
                                if (indexi_choice >= 0 || indexi_choice < length)
                                {
                                    R2 = R2.Insert(string_arri, indexi_choice);      // insert substring into R2
                                    Console.WriteLine("Rope 2 after insertion: ");
                                    R2.Print(R2.Root);
                                }
                                else { Console.WriteLine("ERROR: Index out of range. Returning to main menu...\n"); }
                                break;
                            default:
                                Console.WriteLine("ERROR: Rope does not exist. Returning to main menu...\n");       // print error message
                                break;
                        }
                        Console.WriteLine();
                        break;

                    case 3:     // Delete a substring from an existing rope
                        int_rope_choice = program.RopeChoice();     // get rope
                        Console.WriteLine("First index...");
                        indexi_choice = program.IntChoice();        // get indexes
                        Console.WriteLine("Second index...");
                        indexj_choice = program.IntChoice();
                        switch (int_rope_choice)
                        {
                            case 1:
                                if (R1.Root == null)
                                {
                                    Console.WriteLine("ERROR: Rope is empty. Returning to main menu...\n");
                                    break;
                                }
                                length = R1.GetTotal();
                                if ((indexi_choice >= 0 || indexi_choice < length) && (indexj_choice >= 0 || indexj_choice < length))
                                {
                                    R1 = R1.Delete(indexi_choice, indexj_choice);        // delete the substring from R1
                                    Console.WriteLine("Rope 1 after deletion: ");
                                    R1.Print(R1.Root);
                                }
                                else { Console.WriteLine("ERROR: Index out of range. Returning to main menu...\n"); }
                                break;
                            case 2:
                                if (R2.Root == null)
                                {
                                    Console.WriteLine("ERROR: Rope is empty. Returning to main menu...\n");
                                    break;
                                }
                                length = R2.GetTotal();
                                if ((indexi_choice >= 0 || indexi_choice < length) && (indexj_choice >= 0 || indexj_choice < length))
                                {
                                    R2 = R2.Delete(indexi_choice, indexj_choice);        // delete the substring from R2
                                    Console.WriteLine("Rope 2 after deletion: ");
                                    R2.Print(R2.Root);
                                }
                                else { Console.WriteLine("ERROR: Index out of range. Returning to main menu...\n"); }
                                break;
                            default:
                                Console.WriteLine("ERROR: Rope does not exist. Returning to main menu...\n");       // print error message
                                break;
                        }
                        MyGlobals.FIRST = true;
                        Console.WriteLine();
                        break;

                    case 4:     // Find a character in an existing rope
                        int_rope_choice = program.RopeChoice();     // get rope
                        indexi_choice = program.IntChoice();        // get index
                        switch (int_rope_choice)
                        {
                            case 1:
                                if (R1.Root == null)
                                {
                                    Console.WriteLine("ERROR: Rope is empty. Returning to main menu...\n");
                                    break;
                                }
                                length = R1.GetTotal();
                                if (indexi_choice >= 0 || indexi_choice < length)
                                {
                                    re_ch = R1.CharAt(R1.Root, indexi_choice, 0);       // find the character in R1
                                    Console.WriteLine("Character at " + indexi_choice + ": " + re_ch + "\n");
                                }
                                else { Console.WriteLine("ERROR: Index out of range. Returning to main menu...\n"); }
                                break;
                            case 2:
                                if (R2.Root == null)
                                {
                                    Console.WriteLine("ERROR: Rope is empty. Returning to main menu...\n");
                                    break;
                                }
                                length = R2.GetTotal();
                                if (indexi_choice >= 0 || indexi_choice < length)
                                {
                                    re_ch = R2.CharAt(R2.Root, indexi_choice, 0);       // find the character in R2
                                    Console.WriteLine("Character at " + indexi_choice + ": " + re_ch + "\n");
                                }
                                else { Console.WriteLine("ERROR: Index out of range. Returning to main menu...\n"); }
                                break;
                            default:
                                Console.WriteLine("ERROR: Rope does not exist. Returning to main menu...\n");       // print error message
                                break;
                        }
                        MyGlobals.FIRST = true;
                        break;

                    case 5:     // Find a substring in an existing rope
                        int_rope_choice = program.RopeChoice();     // get rope
                        Console.WriteLine("First index...");
                        indexi_choice = program.IntChoice();        // get indexes
                        Console.WriteLine("Second index...");
                        indexj_choice = program.IntChoice();
                        switch (int_rope_choice)
                        {
                            case 1:
                                if (R1.Root == null)
                                {
                                    Console.WriteLine("ERROR: Rope is empty. Returning to main menu...\n");
                                    break;
                                }
                                length = R1.GetTotal();
                                if ((indexi_choice >= 0 || indexi_choice < length) && (indexj_choice >= 0 || indexj_choice < length))
                                {
                                    re_str = R1.Substring(R1.Root, indexi_choice, indexj_choice, 0);     // Find the substring in R1
                                    Console.WriteLine("Substring from " + indexi_choice + " to " + indexj_choice + ": " + re_str + "\n");
                                }
                                else { Console.WriteLine("ERROR: Index out of range. Returning to main menu...\n"); }
                                break;
                            case 2:
                                if (R2.Root == null)
                                {
                                    Console.WriteLine("ERROR: Rope is empty. Returning to main menu...\n");
                                    break;
                                }
                                length = R2.GetTotal();
                                if ((indexi_choice >= 0 || indexi_choice < length) && (indexj_choice >= 0 || indexj_choice < length))
                                {
                                    re_str = R2.Substring(R2.Root, indexi_choice, indexj_choice, 0);     // Find the substring in R2
                                    Console.WriteLine("Substring from " + indexi_choice + " to " + indexj_choice + ": " + re_str + "\n");
                                }
                                else { Console.WriteLine("ERROR: Index out of range. Returning to main menu...\n"); }
                                break;
                            default:
                                Console.WriteLine("ERROR: Rope does not exist. Returning to main menu...\n");       // print error message
                                break;
                        }
                        MyGlobals.FIRST = true;
                        break;

                    case 6:     // Split a rope into 2 ropes
                        int_rope_choice = program.RopeChoice();     // get rope
                        indexi_choice = program.IntChoice();        // get index
                        switch (int_rope_choice)
                        {
                            case 1:
                                if (R1.Root == null)
                                {
                                    Console.WriteLine("ERROR: Rope is empty. Returning to main menu...\n");
                                    break;
                                }
                                length = R1.GetTotal();
                                if (indexi_choice >= 0 || indexi_choice < length)
                                {
                                    R3 = R1.Split(indexi_choice);       // Split R1 at index
                                    Console.WriteLine("Rope 1 after split: ");
                                    R1.Print(R1.Root);
                                    Console.WriteLine("\nNew rope from split: ");
                                    R3.Print(R3.Root);
                                }
                                else { Console.WriteLine("ERROR: Index out of range. Returning to main menu...\n"); }
                                break;
                            case 2:
                                if (R2.Root == null)
                                {
                                    Console.WriteLine("ERROR: Rope is empty. Returning to main menu...\n");
                                    break;
                                }
                                length = R2.GetTotal();
                                if (indexi_choice >= 0 || indexi_choice < length)
                                {
                                    R3 = R2.Split(indexi_choice);       // Split R2 at index
                                    Console.WriteLine("Rope 2 after split: ");
                                    R2.Print(R2.Root);
                                    Console.WriteLine("New rope from split: ");
                                    R3.Print(R3.Root);
                                }
                                else { Console.WriteLine("ERROR: Index out of range. Returning to main menu...\n"); }
                                break;
                            default:
                                Console.WriteLine("ERROR: Rope does not exist. Returning to main menu...\n");       // print error message
                                break;
                        }
                        Console.WriteLine();
                        break;

                    case 7:     // Concatenate the 2 existing ropes together into a new rope
                        if (R1.Root == null || R2.Root == null)
                        {
                            Console.WriteLine("ERROR: Rope(s) is empty. Returning to main menu...\n");
                            break;
                        }
                        R3 = R1.Concatenate(R2);     // Concatenate ropes R1 & R2, then store as R3
                        Console.Write("Concatenated Strings: ");
                        R3.Print(R3.Root);
                        Console.WriteLine();
                        MyGlobals.FIRST = true;
                        break;

                    case 8:     // Print an existing rope
                        int_rope_choice = program.RopeChoice();     // get rope
                        switch (int_rope_choice)
                        {
                            case 1:
                                if (R1.Root == null)
                                {
                                    Console.WriteLine("ERROR: Rope is empty. Returning to main menu...\n");
                                    break;
                                }
                                R1.Print(R1.Root);      // print Rope R1
                                break;
                            case 2:
                                if (R2.Root == null)
                                {
                                    Console.WriteLine("ERROR: Rope is empty. Returning to main menu...\n");
                                    break;
                                }
                                R2.Print(R2.Root);      // print Rope R2
                                break;
                            default:
                                Console.WriteLine("ERROR: Rope does not exist. Returning to main menu...\n");       // print error message
                                break;
                        }
                        MyGlobals.FIRST = true;
                        Console.WriteLine("\n");
                        break;

                    case 9:     // Exit the program
                        MyGlobals.EXIT = true;
                        Console.WriteLine("Exiting program...");
                        Environment.Exit(0);
                        break;

                    default:    // Invalid integer menu choice
                        Console.WriteLine("ERROR: Invalid option. Please try again.\n");        // print error message
                        break;
                }
            }
        }

        // RopeChoice
        // Receives choice of rope from user and error checks input
        public int RopeChoice()
        {
            bool conversion;
            int int_rope_choice;
            string rope_choice;

            Console.WriteLine("What rope name would you like to use? (1 or 2)");
            do
            {
                rope_choice = Console.ReadLine();       // get user input
                conversion = Int32.TryParse(rope_choice, out int_rope_choice);      // ensure input is an integer
                if (!conversion)        // if input is not an integer...
                {
                    Console.WriteLine("ERROR: Invalid option. Please try again.\n");        // print error message
                }
                else        // if input is an integer...
                {
                    MyGlobals.ERROR = false;        // not an error
                    break;
                }
                Console.WriteLine();
            } while (MyGlobals.ERROR);
            MyGlobals.ERROR = true;
            Console.WriteLine();
            return int_rope_choice;
        }

        // IntChoice
        // Recieves choice of index from user and error checks input
        public int IntChoice()
        {
            int int_choice;
            string choice;
            bool conversion;

            Console.WriteLine("What index are you looking for?");
            do
            {
                choice = Console.ReadLine();        // get user input
                conversion = Int32.TryParse(choice, out int_choice);        // ensure input is an integer
                if (!conversion)        // if input is not an integer...
                {
                    Console.WriteLine("ERROR: Invalid option. Please try again.\n");        // print error message
                }
                else        // if input is an integer...
                {
                    MyGlobals.ERROR = false;        // not an error
                    break;
                }
                Console.WriteLine();
            } while (MyGlobals.ERROR);

            MyGlobals.ERROR = true;
            Console.WriteLine();
            return int_choice;
        }
    }
}
