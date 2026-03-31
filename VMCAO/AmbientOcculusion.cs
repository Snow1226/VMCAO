using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using VMC;
using VMCMod;

namespace VMCAO
{
    [VMCPlugin(
    Name: "VMC Ambient Occulusion",
    Version: "0.0.1",
    Author: "snow",
    Description: "VMCにAmbientOcclusionを追加する",
    AuthorURL: "https://twitter.com/snow_mil",
    PluginURL: "https://github.com/Snow1226/VMCAO")]
    public class AmbientOcculusion : MonoBehaviour
    {
        private PostProcessVolume _ppVolume;
        private PostProcessLayer _ppLayer;
        private AmbientOcclusion _ao;
        private Bloom _bloom;
        private Camera _currentCamera;

        private Settings _settings;

        private bool _displayUI = true;

        private void Awake()
        {
            LoadSetting();
            VMCEvents.OnCameraChanged += OnCameraChanged;
        }

        private void OnDestroy()
        {
            VMCEvents.OnCameraChanged -= OnCameraChanged;
            SaveSetting();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                _displayUI = !_displayUI;
        }

        private void OnCameraChanged(Camera camera)
        {
            if (camera == _currentCamera || _ao != null)  return;
            
            _currentCamera = camera;

            _ppLayer = _currentCamera.gameObject.GetComponent<PostProcessLayer>();

            var ppObj = GameObject.Find("Manager/PostProcessingManager");
            if (ppObj != null)
            {
                _ppVolume = ppObj.GetComponent<PostProcessVolume>();
                var sp = _ppVolume.sharedProfile;

                _ao = sp.GetSetting<AmbientOcclusion>();
                _bloom = sp.GetSetting<Bloom>();
                if (_ao != null)
                    return;
                else
                {
                    _ao = sp.AddSettings<AmbientOcclusion>();

                    if( _ao != null)
                    {
                        _ao.active = true;
                        _ao.enabled.overrideState = true;
                        _ao.enabled.value = _settings.PPS_AO_Enable;
                        _ao.mode.overrideState = true;
                        _ao.mode.value = _settings.PPS_AO_IsScalable ? AmbientOcclusionMode.ScalableAmbientObscurance : AmbientOcclusionMode.MultiScaleVolumetricObscurance;
                        _ao.intensity.overrideState = true;
                        _ao.intensity.value = _settings.PPS_AO_Intensity;
                        _ao.thicknessModifier.overrideState = true;
                        _ao.thicknessModifier.value = _settings.PPS_AO_Thickness;
                        _ao.color.overrideState = true;
                        _ao.color.value = new Color(_settings.PPS_AO_Color_r, _settings.PPS_AO_Color_g, _settings.PPS_AO_Color_b, _settings.PPS_AO_Color_a);
                    }

                }
            }
        }

        private void OnGUI()
        {
            if (_ao != null && _displayUI)
            {
                using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(240)))
                {
                    _ppVolume.enabled = GUILayout.Toggle(_ppVolume.enabled, "Post Process Enable");

                    _ppLayer.enabled = GUILayout.Toggle(_ppLayer.enabled, "Main Camera PostProcess");

                    using (new GUILayout.VerticalScope(GUI.skin.box))
                    {
                        GUILayout.Label("Ambien Occlusion Setting");
                        _ao.enabled.value = GUILayout.Toggle(_ao.enabled.value, "Enable");

                        var isScalable = _ao.mode.value == AmbientOcclusionMode.ScalableAmbientObscurance ? true : false;
                        isScalable = GUILayout.Toggle(isScalable, "Is Scalable");
                        _ao.mode.value = isScalable ? AmbientOcclusionMode.ScalableAmbientObscurance : AmbientOcclusionMode.MultiScaleVolumetricObscurance;

                        GUILayout.Space(10);

                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("Intensity", GUILayout.Width(80));
                            _ao.intensity.value = GUILayout.HorizontalSlider(_ao.intensity.value, 0f, 1f);
                        }
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("Thickness", GUILayout.Width(80));
                            _ao.thicknessModifier.value = GUILayout.HorizontalSlider(_ao.thicknessModifier.value, 0f, 1f);
                        }

                        GUILayout.Space(10);

                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("R:", GUILayout.Width(80));
                            _ao.color.value.r = GUILayout.HorizontalSlider(_ao.color.value.r, 0, 1);
                        }
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("G:", GUILayout.Width(80));
                            _ao.color.value.g = GUILayout.HorizontalSlider(_ao.color.value.g, 0, 1);
                        }
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("B:", GUILayout.Width(80));
                            _ao.color.value.b = GUILayout.HorizontalSlider(_ao.color.value.b, 0, 1);
                        }
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("A:", GUILayout.Width(80));
                            _ao.color.value.a = GUILayout.HorizontalSlider(_ao.color.value.a, 0, 1);
                        }
                    }

                    if (_bloom != null)
                    {
                        using (new GUILayout.VerticalScope(GUI.skin.box))
                        {
                            GUILayout.Label("Bloom Setting");
                            _bloom.enabled.value = GUILayout.Toggle(_bloom.enabled.value, "Enable");
                            GUILayout.Space(10);

                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Label("Intensity", GUILayout.Width(80));
                                _bloom.intensity.value = GUILayout.HorizontalSlider(_bloom.intensity.value, 0f, 10f);
                            }
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Label("Threshold", GUILayout.Width(80));
                                _bloom.threshold.value = GUILayout.HorizontalSlider(_bloom.threshold.value, 0f, 1.5f);
                            }

                            GUILayout.Space(10);

                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Label("R:", GUILayout.Width(80));
                                _bloom.color.value.r = GUILayout.HorizontalSlider(_bloom.color.value.r, 0, 1);
                            }
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Label("G:", GUILayout.Width(80));
                                _bloom.color.value.g = GUILayout.HorizontalSlider(_bloom.color.value.g, 0, 1);
                            }
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Label("B:", GUILayout.Width(80));
                                _bloom.color.value.b = GUILayout.HorizontalSlider(_bloom.color.value.b, 0, 1);
                            }
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Label("A:", GUILayout.Width(80));
                                _bloom.color.value.a = GUILayout.HorizontalSlider(_bloom.color.value.a, 0, 1);
                            }

                        }
                    }

                    using(new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("x", GUILayout.Width(60)))
                        {
                            _displayUI = false;
                        }
                    }
                }

                _settings.PPS_AO_Enable = _ao.enabled;
                _settings.PPS_AO_IsScalable = _ao.mode.value == AmbientOcclusionMode.ScalableAmbientObscurance ? true : false;
                _settings.PPS_AO_Intensity = _ao.intensity.value;
                _settings.PPS_AO_Thickness = _ao.thicknessModifier.value;
                _settings.PPS_AO_Color_r = _ao.color.value.r;
                _settings.PPS_AO_Color_g = _ao.color.value.g;
                _settings.PPS_AO_Color_b = _ao.color.value.b;
                _settings.PPS_AO_Color_a = _ao.color.value.a;
            }
        }

        private void LoadSetting()
        {
            string dllDirectory = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;
            if (File.Exists(Path.Combine(dllDirectory, "VMCAO.json")))
                _settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(Path.Combine(dllDirectory, "VMCAO.json")));
            else
            {
                _settings = new Settings();
                File.WriteAllText(Path.Combine(dllDirectory, "VMCAO.json"), JsonConvert.SerializeObject(_settings, Formatting.Indented));
            }
        }

        private void SaveSetting()
        {
            string dllDirectory = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;
            File.WriteAllText(Path.Combine(dllDirectory, "VMCAO.json"), JsonConvert.SerializeObject(_settings, Formatting.Indented));

        }
    }
}
