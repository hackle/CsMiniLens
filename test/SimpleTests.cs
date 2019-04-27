using System;
using CSharpLens;
using Xunit;
using DeepEqual.Syntax;
using System.Collections.Generic;

namespace CharpLensTest
{
    public class SimpleTests
    {
        public static IEnumerable<object[]> ViewTestCases = new object[][] {
            new object[]{ "null", null, null },
            new object[]{ "empty", new Person(), null },
            new object[]{ 
                "valid", 
                new Person { Address = new Address { Street = "Queen" } },     
                "Queen",
            },
        };

        [Theory]
        [MemberData(nameof(ViewTestCases))]
        public void Get(string note, Person person, string expectedStreet)
        {
            var lens = Lens.For<Person, String>(p => p.Address.Street);

            Assert.Equal(lens.View(person), expectedStreet);
        }

        public static IEnumerable<object[]> SetTestCases = new object[][] {
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
                "valid",
                new Person { Address = new Address { Street = "Queen" } },
                "Sale",
                new Person { Address = new Address { Street = "Sale" } },
            },
        };

        [Theory]
        [MemberData(nameof(SetTestCases))]
        public void Set(string note, Person person, string street, Person expected)
        {
            var lens = Lens.For<Person, String>(p => p.Address.Street);

            lens.Set(person, street).ShouldDeepEqual(expected);
        }
    }
}
