using System;
using System.Collections.Generic;
using LukeSkywalker.IPNetwork;

namespace IPList
{
    public class Lists
    {
        public Lists()
        {
        }

        public static List<List<T>> Split<T>(IPAddressCollection collection, int size = 10)
        {
            // calculate chunk size based on number of IPs and a max of 30 threads
            double col_count = collection.Count;
            double threads = col_count / size;
            while (threads > 20)
            {
                size *= 2;
                threads = col_count / size;
            }

            List<T> CollectionList = new List<T>();
            foreach (var item in collection)
            {
                string[] split_ip = item.ToString().Split(".");
                if (split_ip[3] != "0" && split_ip[3] != "255")
                {
                    CollectionList.Add((T)Convert.ChangeType(item.ToString(), typeof(T)));
                }
            }

            var chunks = new List<List<T>>();
            var temp = new List<T>();
            var count = 0;

            foreach (var element in CollectionList)
            {
                if (count++ == size)
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

        public static List<List<T>> Split<T>(List<int> collection, int size = 10)
        {
            // calculate chunk size based on number of IPs and a max of 30 threads
            double col_count = collection.Count;
            double threads = col_count / size;
            while (threads > 20)
            {
                size *= 2;
                threads = col_count / size;
            }

            List<T> CollectionList = new List<T>();
            foreach (var item in collection)
            {
                CollectionList.Add((T)Convert.ChangeType(item.ToString(), typeof(T)));
            }

            var chunks = new List<List<T>>();
            var temp = new List<T>();
            var count = 0;

            foreach (var element in CollectionList)
            {
                if (count++ == size)
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
