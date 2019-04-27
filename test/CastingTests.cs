using System;
using CSharpLens;
using System.Collections.Generic;
using Xunit;
using DeepEqual.Syntax;

namespace CharpLensTest
{
    public class CastingTests
    {
        public static IEnumerable<object[]> TypeAsViewTestCases = new object[][] {
            new object[] {
                "Null",
                null,
                null,
            },
            new object[] {
                "Empty",
                new Person(),
                null,
            },
            new object[] {
                "Address type no match",
                new Person { Address = new Address { Street = "Queen" } },
                null,
            },
            new object[] {
                "Address type matches",
                new Person { Address = new CityAddress { Street = "Queen" } },
                "Queen",
            },
        };

        [Theory]
        [MemberData(nameof(TypeAsViewTestCases))]
        public void Get(string note, Person person, string expectedStreet)
        {
            var lens = Lens.For<Person, String>(p => (p.Address as CityAddress).Street);

            Assert.Equal(lens.View(person), expectedStreet);
        }

        public static IEnumerable<object[]> TypeAsSetTestCases = new object[][] {
            new object[] {
                "Null",
                null,
                "Sale",
                null,
            },
            new object[] {
                "Empty",
                new Person(),
                "Sale",
                new Person(),
            },
            new object[] {
                "Address type no match",
                new Person { Address = new Address { Street = "Queen" } },
                "Sale",
                new Person { Address = new Address { Street = "Queen" } },
            },
            new object[] {
                "Address type matches",
                new Person { Address = new CityAddress { Street = "Queen" } },
                "Sale",
                new Person { Address = new CityAddress { Street = "Sale" } },
            },
        };

        [Theory]
        [MemberData(nameof(TypeAsSetTestCases))]
        public void Set(string note, Person person, string street, Person expected)
        {
            var lens = Lens.For<Person, String>(p => (p.Address as CityAddress).Street);

            lens.Set(person, street).ShouldDeepEqual(expected);
        }

        [Theory]
        [MemberData(nameof(TypeAsSetTestCases))]
        public void ViewSet(string note, Person person, string streetIgnored, Person personIgnored) {
            var lPAddr = Lens.For<Person, String>(p => p.Address.Street);
            lPAddr.Set(person, lPAddr.View(person)).ShouldDeepEqual(person);
        }

        [Theory(Skip = "Set view won't hold through nullable")]
        [MemberData(nameof(TypeAsSetTestCases))]
        public void SetView(string note, Person person, string street, Person personIgnored) {
            var lPAddr = Lens.For<Person, String>(p => p.Address.Street);
            
            Assert.Equal(street, lPAddr.View(lPAddr.Set(person, street)));
        }

        [Theory]
        [MemberData(nameof(TypeAsSetTestCases))]
        public void SetSet(string note, Person person, string street, Person personIgnored) {
            var lPAddr = Lens.For<Person, String>(p => p.Address.Street);
            
            var set1 = lPAddr.Set(person, street);
            var set2 = lPAddr.Set(set1, street);

            set1.ShouldDeepEqual(set2);
        }
    }
}
