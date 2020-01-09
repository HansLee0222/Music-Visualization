using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoshPit : MonoBehaviour
{   
    public AudioManager audio;
    public int binsPerMoshBin;

    public List<MoshBin> bins;
    public float[] amplitudes;

    public BinMapping mapping;

    public float gain;
    public float minAmp;
    public float equalLoudnessCompensation;
    
    public GameObject moshBinPrefab;
    public GameObject mosherPrefab;

    public Vector2 size;
    private Vector3[] corners = new Vector3[4];
    public float floorHeight;

    public int binCount;
    public int binSize;
    public float minFreq;
    public float maxFreq;
    public int wavelength;

    public float spreadAngle;
    public float inclineAngle;
    public float dispersion;

    //For mapping moshers to bins
    public float angleSpacing_rad; 
    public float binFocus;
    public float binCenterOffset;

    public float phaseRestoreCoeff;
    public float horizRestoreCoeff;
    public float horizRestoreMax;


    void Start(){
        transform.position = new Vector3(-size.x/2, 0, 0);
        binsPerMoshBin = audio.spectrum.Length / binCount;
    	GenerateCrowd();
    }


    public void GenerateCrowd(){
        Vector3 posSpacing = new Vector3(size.x/(binCount-1), 0, 0);
        float angleSpacing = spreadAngle / (binCount-1);
        float freqRatio = (binCount > 1) ? Mathf.Pow(maxFreq/minFreq, 1f/(binCount-1)) : 1;
        
        angleSpacing_rad = angleSpacing*Mathf.Deg2Rad;
        binFocus = Mathf.Tan((Mathf.PI - spreadAngle*Mathf.Deg2Rad)/2)*(size.x/2);
        binCenterOffset = Mathf.PI/2f; //- angleSpacing_rad/2f;
        
        Vector3 pos = transform.position;
        float angle = -spreadAngle/2; 
        float freq = minFreq;

        int totalBoundFreqBins = 0;

    	for(int x = 0; x < binCount; x++){
    		MoshBin bin = Instantiate(moshBinPrefab, pos, Quaternion.identity, transform).GetComponent<MoshBin>();
    	
            int maxBoundFreqBin = MoshUtil.MoshBin2MaxFreqBin(x, binCount, mapping);
            int boundFreqBins;

            if(x == 0){
                boundFreqBins = maxBoundFreqBin;
            }else if(maxBoundFreqBin <= totalBoundFreqBins){
                boundFreqBins = 1;
            }else{
                boundFreqBins = maxBoundFreqBin - totalBoundFreqBins;
            }
            
            totalBoundFreqBins += boundFreqBins;

            float centerFreq = MoshUtil.SRATE/MoshUtil.WINDOW_SIZE * totalBoundFreqBins - (boundFreqBins/2);
            float equalLoudnessGain = (MoshUtil.GetEqualLoudnessGain(centerFreq) - 1) * equalLoudnessCompensation + 1;

            bin.Initialize(this, freq, new Vector2(angle, inclineAngle), boundFreqBins, centerFreq, equalLoudnessGain);
    		bins.Add(bin);

    		pos += posSpacing;
    		angle += angleSpacing;
    		freq = freq*freqRatio;
    	}

        amplitudes = new float[bins.Count];
        for(int i = 0; i < amplitudes.Length; i++){
            amplitudes[i] = 1;
        }
    }

    public void OnDrawGizmos(){
    	Vector3[] corners = new Vector3[4];

    	corners[0] = transform.position;

    }

    void Update(){
        int fftBin = 0;
        for(int m = 0; m < bins.Count; m++){
            MoshBin current = bins[m];

            float ampSum = 0;
            for(int b = 0; b < current.boundFreqBins; b++){
                ampSum += audio.spectrum[fftBin];
                fftBin++;
            }

            amplitudes[m] = Mathf.Max(ampSum/Mathf.Sqrt(current.boundFreqBins)*current.equalLoudnessGain*gain, minAmp);
            //amplitudes[m] = Mathf.Max(ampSum*current.equalLoudnessGain*gain, minAmp);
            current.BinUpdate(amplitudes[m]);
        }

    	for(int i = 0; i < bins.Count; i++){
    		
    	}
    }
}
