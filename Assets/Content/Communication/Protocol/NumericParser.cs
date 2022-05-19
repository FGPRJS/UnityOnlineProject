using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;

namespace Content.Communication.Protocol
{
    public class NumericParser
    {
        /// <summary>
        /// Convert string to Vector3
        /// </summary>
        /// <param name="stringVector"></param>
        /// <returns></returns>
        public static Vector3 ParseVector(string stringVector)
        {
            var regexVector = Regex.Replace(stringVector, "[^0-9,.-]", "");

            var parseVector = regexVector.Split(',');

            Vector3 newVector = new Vector3(
                float.Parse(parseVector[0]),
                float.Parse(parseVector[1]),
                float.Parse(parseVector[2]));

            return newVector;
        }

        public static Quaternion ParseQuaternion(string stringQuaternion)
        {
            var regexQuaternion = Regex.Replace(stringQuaternion, "[^0-9 .-]", "");

            var parseQuaternion = regexQuaternion.Split(' ');

            Quaternion newQuaternion = new Quaternion();
            
            for (int i = 0; i < 4; i++)
            {
                float value;
                
                try
                {
                    value = float.Parse(parseQuaternion[i]);
                }
                catch (Exception)
                {
                    value = 0.0f;
                }

                newQuaternion[i] = value;
            }
            
            return newQuaternion;
        }
    }
}