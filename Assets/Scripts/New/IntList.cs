using System;
using UnityEngine;

public class IntList
{
	private int[] data = new int[128];
	private int num_fields = 0;
	private int num = 0;
	private int cap = 128;
	private int free_element = -1;

	// Creates a new list of elements which each consist of integer fields.
	// 'start_num_fields' specifies the number of integer fields each element has.
	public IntList(int start_num_fields)
	{
		num_fields = start_num_fields;
	}

	// Returns the number of elements in the list.
	public int size()
	{
		return num;
	}

	// Returns the value of the specified field for the nth element.
	public int get(int n, int field)
	{
		Debug.Assert(n >= 0 && n < num && field >= 0 && field < num_fields);
		return data[n * num_fields + field];
	}

	// Sets the value of the specified field for the nth element.
	public void set(int n, int field, int val)
	{
		Debug.Assert(n >= 0 && n < num && field >= 0 && field < num_fields);
		data[n * num_fields + field] = val;
	}

	// Clears the list, making it empty.
	public void clear()
	{
		num = 0;
		free_element = -1;
	}

	// Inserts an element to the back of the list and returns an index to it.
	public int pushBack()
	{
		int new_pos = (num + 1) * num_fields;

		// If the list is full, we need to reallocate the buffer to make room
		// for the new element.
		if (new_pos > cap)
		{
			// Use double the size for the new capacity.
			int new_cap = new_pos * 2;

			// Allocate new array and copy former contents.
			int[] new_array = new int[new_cap];
			Array.Copy(data, 0, new_array, 0, cap);
			data = new_array;

			// Set the old capacity to the new capacity.
			cap = new_cap;
		}
		return num++;
	}

	// Removes the element at the back of the list.
	public void popBack()
	{
		// Just decrement the list size.
		Debug.Assert(num > 0);
		--num;
	}

	// Inserts an element to a vacant position in the list and returns an index to it.
	public int insert()
	{
		// If there's a free index in the free list, pop that and use it.
		if (free_element != -1)
		{
			int index = free_element;
			int pos = index * num_fields;

			// Set the free index to the next free index.
			free_element = data[pos];

			// Return the free index.
			return index;
		}
		// Otherwise insert to the back of the array.
		return pushBack();
	}

	// Removes the nth element in the list.
	public void erase(int n)
	{
		// Push the element to the free list.
		int pos = n * num_fields;
		data[pos] = free_element;
		free_element = n;
	}
}