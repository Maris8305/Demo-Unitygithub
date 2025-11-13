using UnityEngine;

public class TorchFlicker : MonoBehaviour
{
    public Light torchLight;
    public float minIntensity = 2f;
    public float maxIntensity = 4f;
    public float flickerSpeed = 0.1f;
    private float targetIntensity;
   
    void Start()
    {
        
        if (torchLight == null)
            torchLight = GetComponent<Light>();

  
        targetIntensity = Random.Range(minIntensity, maxIntensity);
    }
   
    void Update()
    {
       
        torchLight.intensity = Mathf.Lerp(
            torchLight.intensity,      
            targetIntensity,            
            flickerSpeed               
        );

        if (Mathf.Abs(torchLight.intensity - targetIntensity) < 0.1f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
        }
    }
}