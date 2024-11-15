using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class SettingsDropdown : MonoBehaviour
{
    [SerializeField] RenderPipelineAsset[] qualityLevels;

    [SerializeField] TMP_Dropdown drowdown;

    // Start is called before the first frame update
    void Start()
    {
        drowdown.value = QualitySettings.GetQualityLevel();
    }

    public void ChangeLevel(int value)
    {
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = qualityLevels[value];
    }
}
