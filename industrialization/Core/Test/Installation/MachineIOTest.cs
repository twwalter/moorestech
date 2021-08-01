﻿using System;
using System.Collections.Generic;
using System.Linq;
using industrialization.Core.Electric;
using industrialization.Core.GameSystem;
using industrialization.Core.Installation.Machine.util;
using industrialization.Core.Item;
using industrialization.Core.Test.Installation.Generate;
using NUnit.Framework;

namespace industrialization.Core.Test.Installation
{
    public class MachineIoTest
    {
        [TestCase(true,new int[1]{1}, new int[1]{1})]
        [TestCase(true,new int[2]{100,101}, new int[2]{10,10})]
        [TestCase(true,new int[3]{10,11,15}, new int[3]{10,5,8})]
        [TestCase(false,new int[3]{0,5,1}, new int[3]{10,5,8})]
        [TestCase(false,new int[2]{1,0}, new int[2]{10,5})]
        [TestCase(false,new int[2]{0,0}, new int[2]{10,5})]
        public void MachineInputTest(bool isEquals,int[] id,int[] amount)
        {
            var machine = NormalMachineFactory.Create(4, Int32.MaxValue, new DummyInstallationInventory(1));
            var items = new List<IItemStack>();
            for (int i = 0; i < id.Length; i++)
            {
                items.Add(ItemStackFactory.NewItemStack(id[i], amount[i]));
            }

            foreach (var item in items)
            {
                machine.InsertItem(item);
            }

            if (isEquals)
            {
                Assert.AreEqual(items.ToArray(), machine.InputSlot.ToArray());   
            }
            else
            {
                Assert.AreNotEqual(items.ToArray(), machine.InputSlot.ToArray());
            }
            
        }
        [TestCase(new int[2]{1,1}, new int[2]{1,1}, new int[1]{1}, new int[1]{2})]
        [TestCase(new int[2]{3,1}, new int[2]{1,1}, new int[2]{1,3}, new int[2]{1,1})]
        [TestCase(new int[6]{1,3,1,5,5,0}, new int[6]{1,1,2,6,2,4}, new int[4]{0,1,3,5}, new int[4]{4,3,1,8})]
        public void MachineAddInputTest(int[] id,int[] amount,int[] ansid,int[] ansamount)
        {
            var machine = NormalMachineFactory.Create(4,Int32.MaxValue,new DummyInstallationInventory());
            for (int i = 0; i < id.Length; i++)
            {
                machine.InsertItem(ItemStackFactory.NewItemStack(id[i], amount[i]));
            }

            var ansItem = new List<IItemStack>();
            for (int i = 0; i < ansid.Length; i++)
            {
                ansItem.Add(new ItemStack(ansid[i],ansamount[i]));
            }

            for (int i = 0; i < ansItem.Count; i++)
            {
                var m = machine.InputSlot;
                Console.WriteLine(m[i].ToString()+" "+ansItem[i].ToString());
                Assert.True(ansItem[i].Equals(m[i]));
            }
            
        }
        
        
        //アイテムが通常通り処理されるかのテスト
        [Test]
        public void ItemProcessingTest()
        {
            int seed = 2119350917;
            int recipeNum = 20;

            int cnt = 0;
            var r = RecipeGenerate.MakeRecipe(seed,recipeNum);
            var recipes = MachineIoGenerate.MachineIoTestCase(r, seed);
            foreach (var m in recipes)
            {
                var connect = new DummyInstallationInventory(m.output.Count);
                var machine = NormalMachineFactory.Create(m.installtionId,Int32.MaxValue, connect);

                foreach (var minput in m.input)
                {
                    machine.InsertItem(new ItemStack(minput.Id,minput.Amount));
                }

                var electlic = new ElectricSegment();
                electlic.AddInstallationElectric(machine);
                electlic.AddGenerator(new TestPowerGenerator(1000));
                
                var now = DateTime.Now;
                DateTime endTime = now.AddMilliseconds(m.time*m.CraftCnt);
                while (!(connect.InsertedItems.Count != 0 && m.output[0].Equals(connect.InsertedItems[0])))
                {
                    //クラフト時間が超過したら失敗
                    if (endTime.AddSeconds(0.2) < DateTime.Now) Assert.Fail();
                    GameUpdate.Update();
                }
                //クラフト時間が短かったらアウト
                if (DateTime.Now < endTime.AddSeconds(-0.2)) Assert.Fail();
                
                var remainder = machine.InputSlot;
                var output = connect.InsertedItems;

                Assert.False(output.Count <= 0);

                for (int i = 0; i < output.Count; i++)
                {
                    Assert.True(m.output[i].Equals(output[i]));
                }
                
                var inputRemainder = m.inputRemainder.Where(i => i.Id != NullItemStack.NullItemId).ToList();
                inputRemainder.Sort((a, b) => a.Id - b.Id);
                Console.WriteLine(cnt);
                for (int i = 0; i < remainder.Count; i++)
                {
                    Assert.True(inputRemainder[i].Equals(remainder[i]));
                }

                cnt++;
            }
        }
    }
}