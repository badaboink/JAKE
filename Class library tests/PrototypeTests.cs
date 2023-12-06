using JAKE.classlibrary.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_library_tests
{
    public class PrototypeTests
    {
        [Fact]
        public void Test_Create_Deep_Copy()
        {
            ZombieMinion minion = new ZombieMinion(1, "green");
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            ZombieMinion deepcopy = minion.DeepClone() as ZombieMinion;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            deepcopy.SetId(2);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            deepcopy.SetCurrentPosition(10, 10);

            int originalHash = minion.GetHashCode();
            int copyHash = deepcopy.GetHashCode();

            Assert.NotEqual(copyHash, originalHash);

        }

        [Fact]
        public void Test_Create_Shallow_Copy()
        {
            ZombieMinion minion = new ZombieMinion(1, "green");
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            ZombieMinion deepcopy = minion.ShallowClone() as ZombieMinion;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            int originalHash = minion.GetHashCode();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            int copyHash = deepcopy.GetHashCode();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            Assert.Equal(copyHash, originalHash);
        }
    }
}
