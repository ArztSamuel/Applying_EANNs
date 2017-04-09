/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using System;
using System.Collections.Generic;
#endregion

/// <summary>
/// Class implementing a modified genetic algorithm
/// </summary>
public class GeneticAlgorithm
{
    #region Members
    #region Default Parameters
    /// <summary>
    /// Default min value of inital population parameters.
    /// </summary>
    public const float DefInitParamMin = -1.0f;
    /// <summary>
    /// Default max value of initial population parameters.
    /// </summary>
    public const float DefInitParamMax = 1.0f;

    /// <summary>
    /// Default probability of a parameter being swapped during crossover.
    /// </summary>
    public const float DefCrossSwapProb = 0.6f;

    /// <summary>
    /// Default probability of a parameter being mutated.
    /// </summary>
    public const float DefMutationProb = 0.3f;
    /// <summary>
    /// Default amount by which parameters may be mutated.
    /// </summary>
    public const float DefMutationAmount = 2.0f;
    /// <summary>
    /// Default percent of genotypes in a new population that are mutated.
    /// </summary>
    public const float DefMutationPerc = 1.0f;
    #endregion

    #region Operator Delegates
    /// <summary>
    /// Method template for methods used to initialise the initial population.
    /// </summary>
    /// <param name="initialPopulation">The population to be initialised.</param>
    public delegate void InitialisationOperator(IEnumerable<Genotype> initialPopulation);
    /// <summary>
    /// Method template for methods used to evaluate (or start the evluation process of) the current population.
    /// </summary>
    /// <param name="currentPopulation">The current population.</param>
    public delegate void EvaluationOperator(IEnumerable<Genotype> currentPopulation);
    /// <summary>
    /// Method template for methods used to calculate the fitness value of each genotype of the current population.
    /// </summary>
    /// <param name="currentPopulation"></param>
    public delegate void FitnessCalculation(IEnumerable<Genotype> currentPopulation);
    /// <summary>
    /// Method template for methods used to select genotypes of the current population and create the intermediate population.
    /// </summary>
    /// <param name="currentPopulation">The current population,</param>
    /// <returns>The intermediate population.</returns>
    public delegate List<Genotype> SelectionOperator(List<Genotype> currentPopulation);
    /// <summary>
    /// Method template for methods used to recombine the intermediate population to generate a new population.
    /// </summary>
    /// <param name="intermediatePopulation">The intermediate population.</param>
    /// <returns>The new population.</returns>
    public delegate List<Genotype> RecombinationOperator(List<Genotype> intermediatePopulation, uint newPopulationSize);
    /// <summary>
    /// Method template for methods used to mutate the new population.
    /// </summary>
    /// <param name="newPopulation">The mutated new population.</param>
    public delegate void MutationOperator(List<Genotype> newPopulation);
    /// <summary>
    /// Method template for method used to check whether any termination criterion has been met.
    /// </summary>
    /// <param name="currentPopulation">The current population.</param>
    /// <returns>Whether the algorithm shall be terminated.</returns>
    public delegate bool CheckTerminationCriterion(IEnumerable<Genotype> currentPopulation);
    #endregion

    #region Operator Methods
    /// <summary>
    /// Method used to initialise the initial population.
    /// </summary>
    public InitialisationOperator InitialisePopulation = DefaultPopulationInitialisation;
    /// <summary>
    /// Method used to evaluate (or start the evaluation process of) the current population.
    /// </summary>
    public EvaluationOperator Evaluation = AsyncEvaluation;
    /// <summary>
    /// Method used to calculate the fitness value of each genotype of the current population.
    /// </summary>
    public FitnessCalculation FitnessCalculationMethod = DefaultFitnessCalculation;
    /// <summary>
    /// Method used to select genotypes of the current population and create the intermediate population.
    /// </summary>
    public SelectionOperator Selection = DefaultSelectionOperator;
    /// <summary>
    /// Method used to recombine the intermediate population to generate a new population.
    /// </summary>
    public RecombinationOperator Recombination = DefaultRecombinationOperator;
    /// <summary>
    /// Method used to mutate the new population.
    /// </summary>
    public MutationOperator Mutation = DefaultMutationOperator;
    /// <summary>
    /// Method used to check whether any termination criterion has been met.
    /// </summary>
    public CheckTerminationCriterion TerminationCriterion = null;
    #endregion

    private static Random randomizer = new Random();

    private List<Genotype> currentPopulation;

    /// <summary>
    /// The amount of genotypes in a population.
    /// </summary>
    public uint PopulationSize
    {
        get;
        private set;
    }

    /// <summary>
    /// The amount of generations that have already passed.
    /// </summary>
    public uint GenerationCount
    {
        get;
        private set;
    }

    /// <summary>
    /// Whether the current population shall be sorted before calling the termination criterion operator.
    /// </summary>
    public bool SortPopulation
    {
        get;
        private set;
    }

    /// <summary>
    /// Whether the genetic algorithm is currently running.
    /// </summary>
    public bool Running
    {
        get;
        private set;
    }

    /// <summary>
    /// Event for when the algorithm is eventually terminated.
    /// </summary>
    public event System.Action<GeneticAlgorithm> AlgorithmTerminated;
    /// <summary>
    /// Event for when the algorithm has finished fitness calculation. Given parameter is the
    /// current population sorted by fitness if sorting is enabled (see <see cref="SortPopulation"/>).
    /// </summary>
    public event System.Action<IEnumerable<Genotype>> FitnessCalculationFinished;

    #endregion

    #region Constructors
    /// <summary>
    /// Initialises a new genetic algorithm instance, creating a initial population of given size with genotype
    /// of given parameter count.
    /// </summary>
    /// <param name="genotypeParamCount">The amount of parameters per genotype.</param>
    /// <param name="populationSize">The size of the population.</param>
    /// <remarks>
    /// The parameters of the genotypes of the inital population are set to the default float value.
    /// In order to initialise a population properly, assign a method to <see cref="InitialisePopulation"/>
    /// and call <see cref="Start"/> to start the genetic algorithm.
    /// </remarks>
    public GeneticAlgorithm(uint genotypeParamCount, uint populationSize)
    {
        this.PopulationSize = populationSize;
        //Initialise empty population
        currentPopulation = new List<Genotype>((int) populationSize);
        for (int i = 0; i < populationSize; i++)
            currentPopulation.Add(new Genotype(new float[genotypeParamCount]));

        GenerationCount = 1;
        SortPopulation = true;
        Running = false;
    }
    #endregion

    #region Methods
    public void Start()
    {
        Running = true;

        InitialisePopulation(currentPopulation);

        Evaluation(currentPopulation);
    }

    public void EvaluationFinished()
    {
        //Calculate fitness from evaluation
        FitnessCalculationMethod(currentPopulation);

        //Sort population if flag was set
        if (SortPopulation)
            currentPopulation.Sort();

        //Fire fitness calculation finished event
        if (FitnessCalculationFinished != null)
            FitnessCalculationFinished(currentPopulation);

        //Check termination criterion
        if (TerminationCriterion != null && TerminationCriterion(currentPopulation))
        {
            Terminate();
            return;
        }

        //Apply Selection
        List<Genotype> intermediatePopulation = Selection(currentPopulation);

        //Apply Recombination
        List<Genotype> newPopulation = Recombination(intermediatePopulation, PopulationSize);

        //Apply Mutation
        Mutation(newPopulation);

        
        //Set current population to newly generated one and start evaluation again
        currentPopulation = newPopulation;
        GenerationCount++;

        Evaluation(currentPopulation);
    }

    private void Terminate()
    {
        Running = false;
        if (AlgorithmTerminated != null)
            AlgorithmTerminated(this);
    }

    #region Static Methods
    #region Default Operators
    /// <summary>
    /// Initialises the population by setting each parameter to a random value in the default range.
    /// </summary>
    /// <param name="population">The population to be initialised.</param>
    public static void DefaultPopulationInitialisation(IEnumerable<Genotype> population)
    {
        //Set parameters to random values in set range
        foreach (Genotype genotype in population)
            genotype.SetRandomParameters(DefInitParamMin, DefInitParamMax);
    }

    public static void AsyncEvaluation(IEnumerable<Genotype> currentPopulation)
    {
        //At this point the async evaluation should be started and after it is finished EvaluationFinished should be called
    }

    /// <summary>
    /// Calculates the fitness of each genotype by the formula: fitness = evaluation / averageEvaluation.
    /// </summary>
    /// <param name="currentPopulation">The current population.</param>
    public static void DefaultFitnessCalculation(IEnumerable<Genotype> currentPopulation)
    {
        //First calculate average evaluation of whole population
        uint populationSize = 0;
        float overallEvaluation = 0;
        foreach (Genotype genotype in currentPopulation)
        {
            overallEvaluation += genotype.Evaluation;
            populationSize++;
        }

        float averageEvaluation = overallEvaluation / populationSize;

        //Now assign fitness with formula fitness = evaluation / averageEvaluation
        foreach (Genotype genotype in currentPopulation)
            genotype.Fitness = genotype.Evaluation / averageEvaluation;
    }

    /// <summary>
    /// Only selects the best three genotypes of the current population and copies them to the intermediate population.
    /// </summary>
    /// <param name="currentPopulation">The current population.</param>
    /// <returns>The intermediate population.</returns>
    public static List<Genotype> DefaultSelectionOperator(List<Genotype> currentPopulation)
    {
        List<Genotype> intermediatePopulation = new List<Genotype>();
        intermediatePopulation.Add(currentPopulation[0]);
        intermediatePopulation.Add(currentPopulation[1]);
        intermediatePopulation.Add(currentPopulation[2]);

        return intermediatePopulation;
    }

    /// <summary>
    /// Simply crosses the first with the second genotype of the intermediate population until the new 
    /// population is of desired size.
    /// </summary>
    /// <param name="intermediatePopulation">The intermediate population that was created from the selection process.</param>
    /// <returns>The new population.</returns>
    public static List<Genotype> DefaultRecombinationOperator(List<Genotype> intermediatePopulation, uint newPopulationSize)
    {
        if (intermediatePopulation.Count < 2) throw new ArgumentException("Intermediate population size must be greater than 2 for this operator.");

        List<Genotype> newPopulation = new List<Genotype>();
        while (newPopulation.Count < newPopulationSize)
        {
            Genotype offspring1, offspring2;
            CompleteCrossover(intermediatePopulation[0], intermediatePopulation[1], DefCrossSwapProb, out offspring1, out offspring2);

            newPopulation.Add(offspring1);
            if (newPopulation.Count < newPopulationSize)
                newPopulation.Add(offspring2);
        }

        return newPopulation;
    }

    /// <summary>
    /// Simply mutates each genotype with the default mutation probability and amount.
    /// </summary>
    /// <param name="newPopulation">The mutated new population.</param>
    public static void DefaultMutationOperator(List<Genotype> newPopulation)
    {
        foreach (Genotype genotype in newPopulation)
        {
            if (randomizer.NextDouble() < DefMutationPerc)
                MutateGenotype(genotype, DefMutationProb, DefMutationAmount);
        }
    }
    #endregion

    #region Recombination Operators
    public static void CompleteCrossover(Genotype parent1, Genotype parent2, float swapChance, out Genotype offspring1, out Genotype offspring2)
    {
        //Initialise new parameter vectors
        int parameterCount = parent1.ParameterCount;
        float[] off1Parameters = new float[parameterCount], off2Parameters = new float[parameterCount];

        //Iterate over all parameters randomly swapping
        for (int i = 0; i < parameterCount; i++)
        {
            if (randomizer.Next() < swapChance)
            {
                //Swap parameters
                off1Parameters[i] = parent2[i];
                off2Parameters[i] = parent1[i];
            }
            else
            {
                //Don't swap parameters
                off1Parameters[i] = parent1[i];
                off2Parameters[i] = parent2[i];
            }
        }

        offspring1 = new Genotype(off1Parameters);
        offspring2 = new Genotype(off2Parameters);
    }
    #endregion

    #region Mutation Operators
    /// <summary>
    /// Mutates the given genotype by adding a random value in range [-mutationAmount, mutationAmount] to each parameter with a probability of mutationProb.
    /// </summary>
    /// <param name="genotype">The genotype to be mutated.</param>
    /// <param name="mutationProb">The probability of a parameter being mutated.</param>
    /// <param name="mutationAmount">A parameter may be mutated by an amount in range [-mutationAmount, mutationAmount].</param>
    public static void MutateGenotype(Genotype genotype, float mutationProb, float mutationAmount)
    {
        for (int i = 0; i < genotype.ParameterCount; i++)
        {
            if (randomizer.NextDouble() < mutationProb)
            {
                //Mutate by random amount in range [-mutationAmount, mutationAmoun]
                genotype[i] += (float)(randomizer.NextDouble() * (mutationAmount * 2) - mutationAmount);
            }    
        } 
    }
    #endregion
    #endregion
    #endregion

}
