using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessManager : MonoBehaviour
{
    private PostProcessVolume volume;
    private ColorGrading colorGrading;
    private Bloom bloom;

    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<PostProcessVolume>();
        //colorGrading = volume.profile.GetSetting<ColorGrading>();
        bloom = volume.profile.GetSetting<Bloom>();
    }

    // Update is called once per frame
    void Update()
    {
        bloom.intensity.value += 0.1f;
    }
}
