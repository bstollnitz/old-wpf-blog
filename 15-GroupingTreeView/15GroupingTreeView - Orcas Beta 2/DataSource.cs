using System;
using System.Collections.Generic;
using System.Text;

namespace GroupingTreeView
{
    public class Animal
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Category category;

        public Category Category
        {
            get { return category; }
            set { category = value; }
        }

        public Animal(string name, Category category)
        {
            this.name = name;
            this.category = category;
        }
    }

    public enum Category
    {
        Amphibians,
        Bears,
        BigCats,
        Canines,
        Primates,
        Spiders,
    }

    public class Animals
    {
        private List<Animal> animalList;

        public IEnumerable<Animal> AnimalList
        {
            get { return animalList; }
        }

        public Animals()
        {
            animalList = new List<Animal>();
            animalList.Add(new Animal("California Newt", Category.Amphibians));
            animalList.Add(new Animal("Giant Panda", Category.Bears));
            animalList.Add(new Animal("Coyote", Category.Canines));
            animalList.Add(new Animal("Golden Silk Spider", Category.Spiders));
            animalList.Add(new Animal("Mandrill", Category.Primates));
            animalList.Add(new Animal("Black Bear", Category.Bears));
            animalList.Add(new Animal("Jaguar", Category.BigCats));
            animalList.Add(new Animal("Bornean Gibbon", Category.Primates));
            animalList.Add(new Animal("African Wildcat", Category.BigCats));
            animalList.Add(new Animal("Arctic Fox", Category.Canines));
            animalList.Add(new Animal("Tomato Frog", Category.Amphibians));
            animalList.Add(new Animal("Grizzly Bear", Category.Bears));
            animalList.Add(new Animal("Dingo", Category.Canines));
            animalList.Add(new Animal("Gorilla", Category.Primates));
            animalList.Add(new Animal("Green Tree Frog", Category.Amphibians));
            animalList.Add(new Animal("Bald Uakari", Category.Primates));
            animalList.Add(new Animal("Polar Bear", Category.Bears));
            animalList.Add(new Animal("Black Widow Spider", Category.Spiders));
            animalList.Add(new Animal("Bat-Eared Fox", Category.Canines));
            animalList.Add(new Animal("Cheetah", Category.BigCats));
        }
    }
}
