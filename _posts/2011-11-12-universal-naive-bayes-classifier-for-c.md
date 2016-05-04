---
layout: post
title: Universal Naive Bayes Classifier for C#
date: '2011-11-12T12:46:00.001-08:00'
author: Jan Fajfr
tags:
- Machine Learning
- C#
- Computer Science
modified_time: '2014-06-26T14:43:58.259-07:00'
---
This post is dedicated to describe the internal structure and the
possible use of Naive Bayes classifier implemented in C\#.

I was searching for a machine learning library for C\#, something that
would be equivalent to what WEKA is to Java. I have found
[machine.codeplex.com](http://machine.codeplex.com) but it did not
include the Bayesian classification (the one in which I was interested).
So I decided to implement it into the library.

### How to use it
One of the aims of [machine.codeplex.com](http://machine.codeplex.com)
is to allow the users to use simple POCO's for the classification. This
can be achieved by using the C\# attributes. Take a look at the
following example which treats categorization of payments, based on two
features: Amount and Description.
First this is the Payment POCO object with added attributes:

```csharp
public class Payment
{
    [StringFeature(SplitType = StringType.Word)]
    public String Description { get; set; }

    [Feature]
    public Decimal Amount { get; set; }

    [Label]
    public String Category { get; set; }
}
```

And here is how to train the Naive Bayes classifier using a set of
payments and than classify new payment.

```csharp
var data = Payment.GetData();            
NaiveBayesModel<Payment> model = new NaiveBayesModel<Payment>();
var predictor = model.Generate(data);
var item = predictor.Predict(new Payment { Amount = 110, Description = "SPORT SF - PARIS 18 Rue Fleurus" });
```


After the execution the **item.Category** property should be set to a
value based on the analysis of the previously supplied payments.

### About Naive Bayes classifier
This is just small and simplify introduction, refer to the [Wikipedia
article](http://en.wikipedia.org/wiki/Naive_Bayes_classifier) for more
details about Bayesian classification.

Naive Bayes is a very simple classifier which is based on a simple
premise that all the features (or characteristics) of classified items
are independent. This is not really true in the real life, that is why
the model is called naive.
The total probability of a item having features F1, F2, F3 being of
category "C1" can be expressed as:

```
p(F1,F2,F3|C1) = P(C1)*P(F1|C1)*P(F2|C1)*P(F3|C1)
```

Where P(C1) is the **A priory** probability of item being of category C1
and P(F1|C1) is the **Posteriori** probability of item being of category
C1 when it has feature F1.
That is simple for binary features (like "Tall", "Rich"...). For example
**p(Tall|UngulateAnimal) = 0.8**, says that the posteriori probability for
an animal to be and ungulate is 0.8, when it is a tall animal.

If we have continuous features (just like the "Amount" in the payment
example), the Posteriori probability will be expressed slightly
differently. For example P(Amount=123|Household) = 0.4 - can be
translated as: the probability of the payment being part of my household
payments is 0.4, when the amount was 123\$.

When we classify, we compute the total probability for each category (or
class if you want) and we select the category with maximal probability.
We have to thus iterate over all the categories and all the features of
each item and multiply the probabilities to obtain the probability of
the item being in each class.


### How it works inside
After calling the **Generate** method on the model a
**NaiveBayesPredictor** class is created. This class contains the
**Predict** method to classify new objects.
My model can work with three types of features (or characteristics, or
properties):

-   String properties. These properties have to be converted to a binary
    vectors based on the words which they contain. The classifier builds
    a list of all existing words in the set and then the String feature
    can be represented as a set of binary features. For example if the
    bag of all worlds contains four words: (Hello, World, Is, Cool),
    than the following vector [0,1,0,1] represents text "World Cool".
-   Binary properties. Simple true or false properties
-   Continuous properties. By default these are Double or Decimal
    values, but the list could be extend to other types.

After converting the String features to binary features, we have two
types of features:

-   Binary features
-   Continuous features

As mentioned in the introduction for each feature in the item we have to
compute the A priori and Posteriori probabilities. The following
pseudocode shows how to estimate the values of A priori and Posteriori
probabilities. I use array-like notation, just because I have used
arrays also in the implementation.


### Apriori probability
The computation of Apriori probability will be the same for both type of
features.

```
Apriori[i] = #ItemsOfCategory[i] / #Items
```

### Posteriori probability
The Posteriori for binary features will be estimated:

```
Posteriori[i][j] = #ItemsHavingFeature[j]AndCategory[i] / #ItemsOfCategory[i]
```

And the Posteriori probability for continuous features:

```
Posteriori[i][j] = Normal(Avg[i][j],Variance[i][j],value)
```

Where **Normal** references the [normal probability distribution](http://en.wikipedia.org/wiki/Normal_distribution).
*Avg[i][j]* is the average value of feature "j" for items of category "i". Variance[i][j] is the [variance of feature](http://en.wikipedia.org/wiki/Variance) "j" for items of category "i".

If we want to know the probability of payment with Amount=123 being of category "Food", we have the average of all payments of that category let's say: Avg[Food][Amount] = 80, and we have the Variance[Food][Amount] = 24, then the posteriori probability will be equal: Normal(80, 24, 123).

### What does the classifier need?
The response to this question is quite simple, we need at least 4 structures, the meaning should be clear from the previous explication.

```csharp
public double[][] Posteriori { get; set; }
public double[] Apriori { get; set; }
public double[][] CategoryFeatureAvg { get; set; }
public double[][] CategoryFeatureVariance { get; set; }
```

### And how does it classify?
As said before the classification is a loop for all the categories in
the set. For each category we compute the probability by multiplying
apriori probability with posteriori probability of each feature. As we
have two types of features, the computation differs for both of them.
Take a look at this quite simplified code:


```csharp
public T Predict (T item){
  Vector values; // represents the item as a vector
  foreach (var category in Categories)
  {
      for (var feature in Features)
      {
          if (NaiveBayesModel<t>.ContinuesTypes.Contains(feature.Type))
          {
              var value = values[feature];
              var normalProbability = Helper.Gauss(value, CategoryFeatureAvg[category][j], CategoryFeatureVariance[category][j]);
              probability = probability * normalProbability;
          }

          if (feature.Type == typeof(bool)) //String properties are converted also to binary
          {
              var probabilityValue = Posteriori[category][j];
          }
      }

      if (probability > maxProbability)
      {
          maxProbability = probability;
          maxCategory = category;
      }
  }
  item.SetValue(maxCategory);
}
```

That's all there is to it. Once you understand that we need just 4
arrays, it is just a question of how to fill these arrays, that is not
hard (it should be clear from the previous explication), but it takes
some plumbing and looping over all the items in the learning
collection.

If you would like to see the Source Code - check my fork [machine.codeplex.com](http://machine.codeplex.com/SourceControl/network/Forks/hoonzis/NaiveBayes).
