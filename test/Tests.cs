using System;
using CSharpLens;
using Xunit;
using DeepEqual.Syntax;

namespace test
{
    public class Address 
    {
        public string Street { get; set; }
        public int Number { get; set; }
    }
    public class CityAddress : Address {
        public string City;
    }
    public class Person
    {
        public Address Address { get; set; }
        public string Name { get; set; }
    }
    public class LensTests
    {
        [Fact]
        public void Get()
        {
            var lens = Lens.For<Person, String>(p => p.Address.Street);

            Assert.Equal(lens.View(null), null);
            Assert.Equal(lens.View(new Person()), null);
            Assert.Equal(lens.View(new Person { Address = new Address { Street = "Queen" } }), "Queen");
        }

        [Fact]
        public void Set()
        {
            var lens = Lens.For<Person, String>(p => p.Address.Street);

            Assert.Same(lens.Set(null, "Sale"), null);
            lens.Set(new Person(), "Sale").ShouldDeepEqual(new Person());
            lens.Set(new Person { Address = new Address { Street = "Queen" } }, "Sale")
                .ShouldDeepEqual(new Person { Address = new Address { Street = "Sale" } });
        }

        public class TypeAs 
        {
            [Fact]
            public void Get()
            {
                var lens = Lens.For<Person, String>(p => (p.Address as CityAddress).Street);

                Assert.Equal(lens.View(null), null);

                Assert.Equal(lens.View(new Person()), null);

                Assert.Equal(lens.View(new Person { Address = new Address { Street = "Queen" } }), null);

                Assert.Equal(lens.View(new Person { Address = new CityAddress { Street = "Queen" } }), "Queen");
            }

            [Fact]
            public void Set()
            {
                var lens = Lens.For<Person, String>(p => (p.Address as CityAddress).Street);

                Assert.Equal(lens.Set(null, "Sale"), null);

                lens.Set(new Person(), "Sale").ShouldDeepEqual(new Person());

                lens.Set(new Person { Address = new Address { Street = "Queen" } }, "Sale")
                    .ShouldDeepEqual(new Person { Address = new Address { Street = "Queen" } });

                lens.Set(new Person { Address = new CityAddress { Street = "Queen" } }, "Sale")
                    .ShouldDeepEqual(new Person { Address = new CityAddress { Street = "Sale" } });
            }
        }
    }
}
