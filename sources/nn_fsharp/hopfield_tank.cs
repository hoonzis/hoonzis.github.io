// Traveling Salesman Problem via Hopfield-Tank Neural Network 
// Written by David J. Stein, Esq. See "License.html" for terms and conditions of use. 
// 
// This module contains the customized Hopfield-Tank class. 
 
#region Using declarations 
 
using System; 
using System.Collections; 
 
#endregion 
 
namespace Hopfield_Tank_Neural_Network_Namespace { 
 
	class HopfieldTankNetwork	{ 
 
		#region Variables and initialization code 
 
		public string name = "Sample Network"; 
		public ArrayList cities; 
		public double u0 = 0.001;  // this is a complete guess 
		public double u0variation = 0.001; 
		public double[,] distances; 
		public double[,] u, V, uInitial; 
		public double B, C, D; 
		public double dTimeInterval = 0.0001; 
		public double dNormalize = 0.0; 
		public double E = 100000000000.0; 
 
		public HopfieldTankNetwork()	{ 
			cities = new ArrayList(); 
			B = 1.2; 
			D = 1.5; 
		} 
 
		#endregion 
 
		#region Core functions 
 
		internal void Initialize(int randomize) { 
			// This function resets the network with some random (normalized) u and V values. 
			SetDistances(); 
			// find largest distance to normalize all others 
			for (int i = 0; i < cities.Count; i++) { 
				for (int j = 0; j < cities.Count; j++) { 
					dNormalize = Math.Max(dNormalize, distances[i, j]); 
				} 
			} 
 
			Random r = new Random(System.DateTime.Now.Millisecond + randomize); 
			// set u0 values 
			u = new double[cities.Count, cities.Count]; 
			uInitial = new double[cities.Count, cities.Count]; 
			for (int X = 0; X < cities.Count; X++) { 
				for (int i = 0; i < cities.Count; i++) { 
					double randomu0 = (r.Next(100) / 100.0) * (u0variation * 2.0) - u0variation; 
					u[X,i] = uInitial[X,i] = u0 + randomu0; 
				} 
			} 
			// create place for new V values 
			V = new double[cities.Count, cities.Count]; 
			for (int X = 0; X < cities.Count; X++) { 
				for (int i = 0; i < cities.Count; i++) { 
					V[X,i] = 0.75; 
				} 
			} 
			E = 100000000000.0; 
		} 
 
		internal void Reset() { 
			// This function resets the network with the previous set of random u and V values. 
			for (int X = 0; X < cities.Count; X++) { 
				for (int i = 0; i < cities.Count; i++) { 
					u[X,i] = uInitial[X,i]; 
					V[X,i] = 0.75; 
				} 
			} 
			E = 100000000000.0; 
		} 
 
		internal string Analyze() { 
			// This function moves the network one step closer to a solution. 
			string strStatus = ""; 
 
			// Adjust u values. 
			for (int X = 0; X < cities.Count; X++) { 
				for (int i = 0; i < cities.Count; i++) { 
 
					// first term 
					double ASum = -1.0; 
					for (int j = 0; j < cities.Count; j++) { 
						if (j != i) 
							ASum += 2.0 * Math.Pow(V[X, j], 1); 
					} 
 
					// second term 
					double BSum = -1.0; 
					for (int Y = 0; Y < cities.Count; Y++) { 
						if (Y != X) 
							BSum += 2.0 * Math.Pow(V[Y, i], 1); 
					} 
 
					// third term omitted 
 
					// fourth term 
					double DSum = 0.0; 
					for (int Y = 0; Y < cities.Count; Y++) 
						DSum += 0.9 * (float) (distances[X,Y] / dNormalize) * (V[Y, (cities.Count + i + 1) % cities.Count] + V[Y, (cities.Count + i - 1) % cities.Count]); 
 
 					// calculate du and add to u 
					double dudt = ( - ASum - BSum - DSum); 
					u[X,i] += dudt * dTimeInterval; 
				} 
			} 
 
			// Adjust V values. 
			for (int X = 0; X < cities.Count; X++) { 
				for (int i = 0; i < cities.Count; i++) { 
					V[X,i] = 0.5 * (1.0 + Math.Tanh(u[X,i] / u0)); 
				} 
			} 
 
			// Calculate E. 
			// first term 
			double EASum = 0.0; 
			for (int X = 0; X < cities.Count; X++) { 
				double EARowSum = -1.0; 
				for (int i = 0; i < cities.Count; i++) 
					EARowSum += V[X,i]; 
				EASum += Math.Abs(EARowSum); 
			} 
 
			// second term 
			double EBSum = 0.0; 
			for (int i = 0; i < cities.Count; i++) { 
				double EBColSum = -1.0; 
				for (int X = 0; X < cities.Count; X++) 
					EBColSum += V[X,i]; 
				EBSum += Math.Abs(EBColSum); 
			} 
 
			E = EASum + EBSum; 
 
			return strStatus; 
		} 
 
		public void SetDistances() { 
			// This function calculates distances between all cities in the network. 
			if (cities.Count == 0) 
				distances = null; 
			else { 
				// calculate distances 
				distances = new double[cities.Count, cities.Count]; 
				for (int i = 0; i < cities.Count; i++) { 
					for (int j = 0; j < cities.Count; j++) { 
						distances[i, j] = (int) Math.Sqrt(Math.Pow((((City) cities[i]).x - ((City) cities[j]).x), 2.0) + Math.Pow((((City) cities[i]).y - ((City) cities[j]).y), 2.0)); 
					} 
				} 
			} 
		} 
 
		public int TotalDistance() { 
			// This function calculates the total distance of the current solution. 
			bool error = false; 
			int total = 0; 
			int iNextCity = 0; 
			for (int X = 1; X < cities.Count; X++) { 
				if (V[X, 0] > V[iNextCity, 0]) 
					iNextCity = X; 
			} 
			for (int i = cities.Count - 1; i >= 0; i--) { 
				int iCurrentCity = 0; 
				for (int Y = 1; Y < cities.Count; Y++) { 
					if (V[Y, i] > V[iCurrentCity, i]) 
						iCurrentCity = Y; 
				} 
				if (V[iNextCity, (i + 1) % cities.Count] > 0.999) 
					total += (int) distances[iCurrentCity, iNextCity]; 
				else 
					error = true; 
				iNextCity = iCurrentCity; 
			} 
			return (error ? -1 : total); 
		} 
 
		#endregion 
 
	} 
 
 
	public class City { 
 
		#region Variables and initialization code 
 
		public double x, y; 
		public string name; 
		public City(int x, int y) { 
			this.x = x; 
			this.y = y; 
			this.name = "Cleveland"; 
		} 
 
		#endregion 
 
	} 
} 