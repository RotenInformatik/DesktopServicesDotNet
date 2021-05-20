using System;
using System.Collections.Generic;




namespace RI.DesktopServices.Utilities
{
    internal static class TypeExtensions
    {
        public static bool GetBestMatchingType(this Type type, out Type matchingType, out int inheritanceDepth, params Type[] types)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            matchingType = null;
            inheritanceDepth = -1;

            if (types == null)
            {
                return false;
            }

            if (types.Length == 0)
            {
                return false;
            }

            List<Type> inheritance = type.GetInheritance(true);
            inheritance.Reverse();

            List<int> depths = new List<int>();

            foreach (Type candidate in types)
            {
                int depth = -1;

                for (int i1 = 0; i1 < inheritance.Count; i1++)
                {
                    if (inheritance[i1] == candidate)
                    {
                        depth = i1;
                        break;
                    }
                }

                depths.Add(depth);
            }

            int minDepth = int.MaxValue;
            int minIndex = -1;

            for (int i1 = 0; i1 < depths.Count; i1++)
            {
                int depth = depths[i1];

                if ((depth < minDepth) && (depth != -1))
                {
                    minDepth = depth;
                    minIndex = i1;
                }
            }

            if (minIndex == -1)
            {
                return false;
            }

            matchingType = types[minIndex];
            inheritanceDepth = depths[minIndex];
            return true;
        }

        public static List<Type> GetInheritance(this Type type, bool includeSelf)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            List<Type> types = new List<Type>();

            if (includeSelf)
            {
                types.Add(type);
            }

            while (type.BaseType != null)
            {
                type = type.BaseType;
                types.Add(type);
            }

            types.Reverse();

            return types;
        }
    }
}
