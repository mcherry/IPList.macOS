using System;
using System.Collections.Generic;
using System.Net;
using IPAddressCollection = LukeSkywalker.IPNetwork.IPAddressCollection;

namespace IPList
{
    public class Lists
    {
        public Lists() { }

        public static List<List<T>> Split<T>(IPAddressCollection collection, int list_size = 10)
        {
            // calculate chunk size based on number of IPs and a max of 30 threads
            double col_count = collection.Count;
            double lists = col_count / list_size;
            while (lists > 20)
            {
                list_size *= 2;
                lists = col_count / list_size;
            }

            List<T> CollectionList = new List<T>();
            foreach (IPAddress item in collection)
            {
                string[] split_ip = item.ToString().Split(".");
                if (split_ip[3] != "0" && split_ip[3] != "255")
                {
                    CollectionList.Add((T)Convert.ChangeType(item.ToString(), typeof(T)));
                }
            }

            List<List<T>> chunks = new List<List<T>>();
            List<T> temp = new List<T>();
            int count = 0;

            foreach (T element in CollectionList)
            {
                if (count++ == list_size)
                {
                    chunks.Add(temp);
                    temp = new List<T>();
                    count = 1;
                }
                temp.Add(element);
            }

            chunks.Add(temp);
            return chunks;
        }

        public static List<List<T>> Split<T>(List<int> collection, int list_size = 10)
        {
            // calculate chunk size based on number of IPs and a max of 30 threads
            double col_count = collection.Count;
            double lists = col_count / list_size;
            while (lists > 20)
            {
                list_size *= 2;
                lists = col_count / list_size;
            }

            List<T> CollectionList = new List<T>();
            foreach (int item in collection)
            {
                CollectionList.Add((T)Convert.ChangeType(item.ToString(), typeof(T)));
            }

            List<List<T>> chunks = new List<List<T>>();
            List<T> temp = new List<T>();
            int count = 0;

            foreach (T element in CollectionList)
            {
                if (count++ == list_size)
                {
                    chunks.Add(temp);
                    temp = new List<T>();
                    count = 1;
                }
                temp.Add(element);
            }

            chunks.Add(temp);
            return chunks;
        }
    }
}
