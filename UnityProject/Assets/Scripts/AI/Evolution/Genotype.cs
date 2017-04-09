/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
#endregion

/// <summary>
/// Class representing one member of a population
/// </summary>
public class Genotype : IComparable<Genotype>, IEnumerable<float>
{
    #region Members
    private static Random randomizer = new Random();

    /// <summary>
    /// The current evaluation of this genotype.
    /// </summary>
    public float Evaluation
    {
        get;
        set;
    }
    /// <summary>
    /// The current fitness (e.g, the evaluation of this genotype relative 
    /// to the average evaluation of the whole population) of this genotype.
    /// </summary>
    public float Fitness
    {
        get;
        set;
    }

    // The vector of parameters of this genotype.
    private float[] parameters;

    /// <summary>
    /// The amount of parameters stored in the parameter vector of this genotype.
    /// </summary>
    public int ParameterCount
    {
        get
        {
            if (parameters == null) return 0;
            return parameters.Length;
        }
    }

    // Overridden indexer for convenient parameter access.
    public float this[int index]
    {
        get { return parameters[index]; }
        set { parameters[index] = value; }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Instance of a new genotype with given parameter vector and initial fitness of 0.
    /// </summary>
    /// <param name="parameters">The parameter vector to initialise this genotype with.</param>
    public Genotype(float[] parameters)
    {
        this.parameters = parameters;
        Fitness = 0;
    }
    #endregion

    #region Methods
    #region IComparable
    /// <summary>
    /// Compares this genotype with another genotype depending on their fitness values.
    /// </summary>
    /// <param name="other">The genotype to compare this genotype with.</param>
    /// <returns>The result of comparing the two floating point values representing the genotypes fitness in reverse order.</returns>
    public int CompareTo(Genotype other)
    {
        return other.Fitness.CompareTo(this.Fitness); //in reverse order for larger fitness being first in list
    }
    #endregion

    #region IEnumerable
    /// <summary>
    /// Gets an Enumerator to iterate over all parameters of this genotype.
    /// </summary>
    public IEnumerator<float> GetEnumerator()
    {
        for (int i = 0; i < parameters.Length; i++)
            yield return parameters[i];
    }

    /// <summary>
    /// Gets an Enumerator to iterate over all parameters of this genotype.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        for (int i = 0; i < parameters.Length; i++)
            yield return parameters[i];
    }
    #endregion

    /// <summary>
    /// Sets the parameters of this genotype to random values in given range.
    /// </summary>
    /// <param name="minValue">The minimum inclusive value a parameter may be initialised with.</param>
    /// <param name="maxValue">The maximum exclusive value a parameter may be initialised with.</param>
    public void SetRandomParameters(float minValue, float maxValue)
    {
        //Check arguments
        if (minValue > maxValue) throw new ArgumentException("Minimum value may not exceed maximum value.");

        //Generate random parameter vector
        float range = maxValue - minValue;
        for (int i = 0; i < parameters.Length; i++)
            parameters[i] = (float)((randomizer.NextDouble() * range) + minValue); //Create a random float between minValue and maxValue
    }

    /// <summary>
    /// Returns a copy of the parameter vector.
    /// </summary>
    public float[] GetParameterCopy()
    {
        float[] copy = new float[ParameterCount];
        for (int i = 0; i < ParameterCount; i++)
            copy[i] = parameters[i];

        return copy;
    }

    /// <summary>
    /// Saves the parameters of this genotype to a file at given file path.
    /// </summary>
    /// <param name="filePath">The path of the file to save this genotype to.</param>
    /// <remarks>This method will override existing files or attempt to create new files, if the file at given file path does not exist.</remarks>
    public void SaveToFile(string filePath)
    {
        StringBuilder builder = new StringBuilder();
        foreach (float param in parameters)
            builder.Append(param.ToString()).Append(";");

        builder.Remove(builder.Length - 1, 1);

        File.WriteAllText(filePath, builder.ToString());
    }

    #region Static Methods
    /// <summary>
    /// Generates a random genotype with parameters in given range.
    /// </summary>
    /// <param name="parameterCount">The amount of parameters the genotype consists of.</param>
    /// <param name="minValue">The minimum inclusive value a parameter may be initialised with.</param>
    /// <param name="maxValue">The maximum exclusive value a parameter may be initialised with.</param>
    /// <returns>A genotype with random parameter values</returns>
    public static Genotype GenerateRandom(uint parameterCount, float minValue, float maxValue)
    {
        //Check arguments
        if (parameterCount == 0) return new Genotype(new float[0]);

        Genotype randomGenotype = new Genotype(new float[parameterCount]);
        randomGenotype.SetRandomParameters(minValue, maxValue);

        return randomGenotype;
    }

    /// <summary>
    /// Loads a genotype from a file with given file path.
    /// </summary>
    /// <param name="filePath">The path of the file to load the genotype from.</param>
    /// <returns>The genotype loaded from the file at given file path.</returns>
    public static Genotype LoadFromFile(string filePath)
    {
        string data = File.ReadAllText(filePath);

        List<float> parameters = new List<float>();
        string[] paramStrings = data.Split(';');

        foreach (string parameter in paramStrings)
        {
            float parsed;
            if (!float.TryParse(parameter, out parsed)) throw new ArgumentException("The file at given file path does not contain a valid genotype serialisation.");
            parameters.Add(parsed);
        }

        return new Genotype(parameters.ToArray());
    }
    #endregion
    #endregion
}
