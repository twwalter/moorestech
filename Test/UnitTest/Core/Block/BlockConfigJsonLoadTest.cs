using Core.Block.Config;
using NUnit.Framework;
using Test.TestConfig;

namespace Test.UnitTest.Core.Block
{
    public class BlockConfigJsonLoadTest
    {
        /// <summary>
        /// Unit Test Block Config.jsonに基づいてテストコードが書かれています。
        /// 仕様を追加したら、このテストコードを更新してください。
        /// </summary>
        [Test]
        public void JsonLoadTest()
        {
            var path = new TestConfigPath().GetPath("Unit Test Block Config.json");
            var data = new BlockConfigJsonLoad(path).LoadJson();
            
            Assert.AreEqual(data[1].Id,1);
            Assert.AreEqual(data[1].Name,"TestMachine1");
            Assert.AreEqual(data[1].Type,"Machine");
            Assert.AreEqual(((MachineBlockConfigParam)data[1].Param).InputSlot,2);
            Assert.AreEqual(((MachineBlockConfigParam)data[1].Param).OutputSlot,1);
            
            Assert.AreEqual(data[2].Id,2);
            Assert.AreEqual(data[2].Name,"TestMachine2");
            Assert.AreEqual(data[2].Type,"Machine");
            Assert.AreEqual(((MachineBlockConfigParam)data[2].Param).InputSlot,3);
            Assert.AreEqual(((MachineBlockConfigParam)data[2].Param).OutputSlot,1);
            
            Assert.AreEqual(data[3].Id,3);
            Assert.AreEqual(data[3].Name,"TestMachine3");
            Assert.AreEqual(data[3].Type,"Machine");
            Assert.AreEqual(((MachineBlockConfigParam)data[3].Param).InputSlot,2);
            Assert.AreEqual(((MachineBlockConfigParam)data[3].Param).OutputSlot,3);
            
            Assert.AreEqual(data[10].Id,10);
            Assert.AreEqual(data[10].Name,"TestMachine10");
            Assert.AreEqual(data[10].Type,"Machine");
            Assert.AreEqual(((MachineBlockConfigParam)data[10].Param).InputSlot,5);
            Assert.AreEqual(((MachineBlockConfigParam)data[10].Param).OutputSlot,1);
            
            Assert.AreEqual(data[11].Id,11);
            Assert.AreEqual(data[11].Name,"TestMachine11");
            Assert.AreEqual(data[11].Type,"Machine");
            Assert.AreEqual(((MachineBlockConfigParam)data[11].Param).InputSlot,2);
            Assert.AreEqual(((MachineBlockConfigParam)data[11].Param).OutputSlot,6);
        }
    }
}