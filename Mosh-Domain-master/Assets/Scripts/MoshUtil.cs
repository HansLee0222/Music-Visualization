using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BinMapping{
	Linear,
	MelScale,
	ConstantQ
}

public static class MoshUtil {

	public static int SRATE = 44100;
	public static int WINDOW_SIZE = 1024;
	public static int MEL_CONST_1 = 2595;
	public static int MEL_CONST_2 = 700;
	public static int HZ_MAX = 20000;
	public static float MEL_MAX = 3785.166f;

	//copied from https://plot.ly/~mrlyule/16/equal-loudness-contours-iso-226-2003/#data
	public static float[] equalLoudnessGains = {1.122018f, 1.759948f, 3.419795f, 6.471427f, 11.31097f, 19.43121f, 32.99892f, 52.96631f, 81.84645f, 128.0855f, 187.0681f, 264.5452f, 366.0159f, 493.742f, 616.595f, 750.758f, 869.9611f, 874.9836f, 710.3952f, 656.1451f, 957.194f, 1309.182f, 1452.111f, 1288.249f, 874.9836f, 447.7131f, 225.1645f, 169.2388f, 233.3457f, 221.0549f, 2.013725f};
	public static int[] equalLoudnessFreqs = {20, 25, 32, 40, 50, 63, 80, 100, 125, 160, 200, 250, 315, 400, 500, 630, 800, 1000, 1250, 1600, 2000, 2500, 3150, 4000, 5000, 6300, 8000, 10000, 12500, 16000, 20000};

	public static void ParabolicSineParams(float amp, float freq, out float a, out float b){
		amp = 4f*amp;
		float period = 1f/freq;

		a = amp / period;
		b = amp * -2 / (period*period);
	}

	//Apprixomates a rectified sine funtion with a given period.
	//Accepts a t value of the form (Time.time + phase). 
	//Outputs a value between 0 and 1, and from which half of the parabola the value originates.
	//To actually comprehend what this does, see the Mosher class.
	public static Vector2 ParabolicSine(float period, float t){
		t = (t % period)/period;
		float polarity = t < 0.5f ? -1 : 1;

		return new Vector2(4*(t - Mathf.Pow(t, 2)), polarity); 
	}

	public static float Hz2Mels(float hz){
		return 2595 * Mathf.Log(1 + hz/700, 10) - 31.748f;
	}

	public static int MoshBin2MaxFreqBin(int moshBinNum, int moshBinsTotal, BinMapping mapping){
		switch(mapping){
			case BinMapping.Linear:
				return (int)Mathf.Max((float)(moshBinNum + 1) / (moshBinsTotal + 1) * WINDOW_SIZE, 1);

			case BinMapping.MelScale:
				return (int)((Mathf.Pow(10, (float)(moshBinNum + 1) / (moshBinsTotal + 1) * MEL_MAX / MEL_CONST_1) - 1) * MEL_CONST_2 * WINDOW_SIZE / HZ_MAX);

			case BinMapping.ConstantQ:
				return (int)Mathf.Max(Mathf.Pow(WINDOW_SIZE, (float)(moshBinNum + 1) / (moshBinsTotal + 2)), 1);

			default:
				return 0;
		}
		
	}

	public static float GetEqualLoudnessGain(float freq){
		for(int i = 0; i < equalLoudnessFreqs.Length; i++){
			if(freq < equalLoudnessFreqs[i]){
				if(i == 0){ return equalLoudnessGains[0]; }

				//Linear interpolation between data points
				float totalDist = equalLoudnessFreqs[i] - equalLoudnessFreqs[i-1];
				float normalForwardDist = (equalLoudnessFreqs[i] - freq) / totalDist;
				
				return equalLoudnessGains[i-1]*(normalForwardDist) + equalLoudnessGains[i]*(1-normalForwardDist);
			}
		}

		return equalLoudnessGains[equalLoudnessGains.Length-1];
	}
    
}
