﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace industrialization_test.CombinedTest.Core.Generate
{
    public static class RecipeGenerate
    {

        public static recipe MakeRecipe(int seed,int recipeNum)
        {
            var random = new Random(seed);
            var recipes = new List<recipes>();
            for (int i = 0; i < recipeNum; i++)
            {
                recipes.Add(new recipes(random));
            }

            return new recipe(recipes.ToArray());
        }
    }

        [DataContract]
        public class recipe
        {
            [DataMember(Name = "recipes")]
            public recipes[] recipes;

            public recipe(recipes[] recipes)
            {
                this.recipes = recipes;
            }
        }

        [DataContract]
        public class recipes
        {
            [DataMember(Name = "BlockID")]
            public int BlockID;
            [DataMember(Name = "time")]
            public int time;
            [DataMember(Name = "input")]
            public inputitem[] input;
            [DataMember(Name = "output")]
            public outputitem[] output;

            public recipes(Random r)
            {
                int inputnum = r.Next(1, 11);
                var tmpInput = new List<inputitem>();
                for (int i = 0; i < inputnum; i++)
                {
                    tmpInput.Add(new inputitem(r.Next(0,1001),r.Next(1,101)));
                }
                input = tmpInput.ToArray();
                
                int outputnum = r.Next(1, 11);
                var tmpOutput = new List<outputitem>();
                for (int i = 0; i < outputnum; i++)
                {
                    tmpOutput.Add(new outputitem(r.Next(0,1001),r.Next(1,101),1));
                }
                output = tmpOutput.ToArray();
                
                
                BlockID = r.Next(0,101);
                time = r.Next(1,4)*1000;
            }
        }

        [DataContract]
        public class inputitem
        {
            [DataMember(Name = "id")]
            public int id;
            [DataMember(Name = "amount")]
            public int amount;

            public inputitem(int id, int amount)
            {
                this.id = id;
                this.amount = amount;
            }
        }

        [DataContract]
        public class outputitem
        {
            [DataMember(Name = "id")]
            public int id;
            [DataMember(Name = "amount")]
            public int amount;
            [DataMember(Name = "percent")]
            public double percent;

            public outputitem(int id, int amount, double percent)
            {
                this.id = id;
                this.amount = amount;
                this.percent = percent;
            }
        }
}