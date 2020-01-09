using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoshBin : MonoBehaviour
{
	public Mosher[] moshers;
	public MoshPit pit;
    public int boundFreqBins;

	public float freq;
	public float period;
	public Vector2 angle;
	public Vector3 direction;

	public float gravity;
	public float jumpSpeed;
    public int wavelength;

    public float centerFreq;
    public float equalLoudnessGain;

    public void Initialize(MoshPit p, float f, Vector2 a, int b, float c, float g){
    	pit = p;
    	moshers = new Mosher[pit.binSize];
        boundFreqBins = b;

        centerFreq = c;
        equalLoudnessGain = g;

    	freq = f;
    	period = 1f/freq;
    	angle = a;
    	direction = new Vector3(Mathf.Sin(angle.x*Mathf.Deg2Rad), Mathf.Sin(angle.y*Mathf.Deg2Rad), Mathf.Cos(angle.x*Mathf.Deg2Rad)).normalized;

    	float amplitude = 5f; //TEMP
        wavelength = pit.wavelength;

    	MoshUtil.ParabolicSineParams(amplitude, freq, out jumpSpeed, out gravity);
    	
    	GenerateMoshers();
    }

    private void GenerateMoshers(){
        Vector3 posSpacing = direction*(pit.size.y/moshers.Length);
        float phaseSpacing = period/wavelength;

        Vector3 pos = transform.position;
        float phase = 0;
        Quaternion rotation = Quaternion.Euler(0, angle.x, 0);

        Vector3 dispersionConst = Mathf.Sin(pit.angleSpacing_rad) * new Vector3(direction.z, 0, -direction.x) * pit.dispersion;
        float distFromFocus = (transform.position - new Vector3(0, 0, -pit.binFocus)).magnitude;

    	for(int z = 0; z < moshers.Length; z++){
            Vector3 offset = (Random.value-0.5f)*dispersionConst*(z+distFromFocus);

    		moshers[z] = Instantiate(pit.mosherPrefab, pos + offset, rotation, transform).GetComponent<Mosher>();
            moshers[z].Initialize(this, phase, gravity, pos.y);
    		pos += posSpacing;
            phase = phase - phaseSpacing;

            if(phase >= period){ phase = 0; }
    	}
    }


    //Returns some measure of the bin's energy
    public float BinUpdate(float amp){
        MoshUtil.ParabolicSineParams(amp, freq, out jumpSpeed, out gravity);

    	Mosher m;
    	bool jumped;
    	for(int i = 0; i < moshers.Length; i++){
    		m = moshers[i];
    		jumped = m.MosherUpdate();

    		if(jumped){
    			//m.phaseError = currentBouncePoints[i/wavelength] - i;
    		}
    	}

    	return 0f;
    }

}
