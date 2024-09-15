using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using VIRTUE;

public class DivisorFinder : MonoBehaviour {
    public int numberToFindDivisorsOf = 12;

    [Button]
    void Find () {
        List<int> divisors = FindDivisors (numberToFindDivisorsOf);
        Debug.Log ("Divisors of " + numberToFindDivisorsOf + " are: ");
        /*foreach (int divisor in divisors) {
            Debug.Log (divisor);
        }*/
        var size = divisors.Count / 2;
        for (int i = 0, j = divisors.Count - 1; i < size; i++, j--) {
            this.Log ("-> ", divisors[i], " => ", divisors[j]);
        }
    }

    List<int> FindDivisors (int number) {
        List<int> divisors = new List<int> ();
        for (int i = 1; i <= number; i++) {
            if (number % i == 0) {
                divisors.Add (i);
            }
        }
        return divisors;
    }
}