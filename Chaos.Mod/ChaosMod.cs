using System;
using System.Collections.Generic;
using Chaos.Engine.Systems;
using Chaos.Mod.Content.Renderers;
using Chaos.Mod.Content.Renderers.Enums;
using Chaos.Mod.Content.Renderers.Shaders;
using Chaos.Mod.Content.Tasks;
using Chaos.Mod.Controllers;
using VintageMods.Core.Attributes;
using VintageMods.Core.IO.Enum;
using VintageMods.Core.ModSystems;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

[assembly: ModDomain("chaosmod", "ChaosMod")]

namespace Chaos.Mod
{
    internal class ChaosMod : UniversalInternalMod<ServerSystemChaosMod, ClientSystemChaosMod>
    {
        private OverlayRenderer _overlayRenderer;
        private OverlayShaderProgram _prog;

        public ChaosMod() : base("chaosmod")
        {
        }

        public EffectsController Effects { get; private set; }

        public override double ExecuteOrder()
        {
            return 0.0;
        }

        // Perform actions on both Server and Client.
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            Files.RegisterFile("global-config.json", FileScope.Global);
            Effects = new EffectsController();
        }

        // Perform actions on Client.
        public override void StartClientSide(ICoreClientAPI capi)
        {
            Effects.InitialiseClient(capi);
            RegisterHotkeys();
        }

        private bool LoadShader()
        {
            Capi.Shader.RegisterFileShaderProgram("colour-overlay", _prog);
            _prog.Compile();
            if (_overlayRenderer != null) _overlayRenderer.Shader = _prog;
            return true;
        }


        // Perform actions on Server.
        public override void StartServerSide(ICoreServerAPI sapi)
        {
            Effects.InitialiseServer(sapi);
            sapi.RegisterCommand("storm", "", "", (_, _, _) =>
            {
                var currentTime = sapi.World.Calendar.TotalDays;
                var system = sapi.ModLoader.GetModSystem<SystemTemporalStability>();
                system.StormData.nextStormTotalDays = currentTime;
            });
            AiTaskRegistry.Register<AiTaskDance>("dance");
        }

        private bool ExecuteCommand(KeyCombination _)
        {
            var effects = new Dictionary<string, List<string>>
            {
                {
                    "Block", new()
                    {
                        ""
                    }
                },

                {
                    "Creature", new()
                    {
                        "AllNearbyDriftersDance",
                        "ObliterateAllNearbyAnimals",
                        "ReviveAllNearbyCorpses",
                        "YeetAllNearbyCreatures"
                    }
                },

                {
                    "Item", new()
                    {
                        ""
                    }
                },

                {
                    "Meta", new()
                    {
                        ""
                    }
                },

                {
                    "Misc", new()
                    {
                        "Nothing",
                        "QuakeProFOV",
                        "WhatAustraliaLooksLike",
                        "ZoomZoomCam"
                    }
                },

                {
                    "Player", new()
                    {
                        "Forcefield",
                        "HealToFull",
                        "ThermonuclearHandGrenade",
                        "Yeet"
                    }
                },

                {
                    "Shader", new()
                    {
                        "AcidTrip",
                        "AlienWorld",
                        "AntiqueStory",
                        "BlueLightDistrict",
                        "CatsEyes",
                        "DeepFried",
                        "NightVision",
                        "SeeingRed",
                        "VSNoire"
                    }
                },

                {
                    "Time", new()
                    {
                        ""
                    }
                },

                {
                    "Weather", new()
                    {
                        "StartTemporalStorm"
                    }
                }
            };
            Effects.StartExecuteClient("Yeet");
            return true;
        }

        #region Sandbox Methods

        private void RegisterHotkeys()
        {
            #region Keypad Hotkey Handlers

            //
            // NumPad 0
            //

            Capi.Input.RegisterHotKey("sb0", "SandboxTest 0", GlKeys.Keypad0);
            Capi.Input.SetHotKeyHandler("sb0", _ =>
            {
                _prog ??= new OverlayShaderProgram {Name = "colour-overlay"};
                _overlayRenderer ??= new OverlayRenderer
                {
                    Shader = _prog,
                    LightReactive = false,
                    BlendMode = EnumBlendMode.Overlay
                };

                if (_overlayRenderer is {Active: true})
                {
                    _overlayRenderer.Active = false;
                    Capi.Event.ReloadShader -= LoadShader;
                    Capi.Event.UnregisterRenderer(_overlayRenderer, EnumRenderStage.AfterFinalComposition);
                    Capi.Shader.ReloadShaders();
                    Capi.ShowChatMessage("Renderer Unregistered.");
                    _overlayRenderer = null;
                    _prog = null;
                }
                else
                {
                    Capi.Event.RegisterRenderer(_overlayRenderer, EnumRenderStage.AfterFinalComposition);
                    Capi.Event.ReloadShader += LoadShader;
                    LoadShader();
                    _overlayRenderer.Active = true;
                    Capi.ShowChatMessage("Renderer Registered.");
                }

                return true;
            });

            //
            // NumPad 1
            //

            Capi.Input.RegisterHotKey("sb1", "SandboxTest 1", GlKeys.Keypad1);
            Capi.Input.SetHotKeyHandler("sb1", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                var current = (int) _prog.Filter;
                _prog.Filter = (OverlayColourFilter) GameMath.Clamp(current + 1, 0, 
                    Enum.GetNames(typeof(OverlayColourFilter)).Length -1);
                Capi.ShowChatMessage($"Filter: {_prog.Filter}");
                return true;
            });

            Capi.Input.RegisterHotKey("ssb1", "SandboxTest 1", GlKeys.Keypad1, HotkeyType.CharacterControls, true);
            Capi.Input.SetHotKeyHandler("ssb1", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                var current = (int) _prog.Filter;
                _prog.Filter = (OverlayColourFilter) GameMath.Clamp(current - 1, 0, 
                    Enum.GetNames(typeof(OverlayColourFilter)).Length -1);
                Capi.ShowChatMessage($"Filter: {_prog.Filter}");

                return true;
            });

            Capi.Input.RegisterHotKey("csb1", "SandboxTest 1", GlKeys.Keypad1, HotkeyType.CharacterControls, false,
                true);
            Capi.Input.SetHotKeyHandler("csb1", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Filter = OverlayColourFilter.None;
                Capi.ShowChatMessage($"Filter: {_prog.Filter}");

                return true;
            });

            //
            // NumPad 2
            //

            Capi.Input.RegisterHotKey("sb2", "SandboxTest 2", GlKeys.Keypad2);
            Capi.Input.SetHotKeyHandler("sb2", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Saturation = GameMath.Clamp(_prog.Saturation += 0.1f, 0, 100);
                Capi.ShowChatMessage($"Saturation: {_prog.Saturation}");
                return true;
            });

            Capi.Input.RegisterHotKey("ssb2", "SandboxTest 2", GlKeys.Keypad2, HotkeyType.CharacterControls, true);
            Capi.Input.SetHotKeyHandler("ssb2", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Saturation = GameMath.Clamp(_prog.Saturation -= 0.1f, 0, 100);
                Capi.ShowChatMessage($"Saturation: {_prog.Saturation}");
                return true;
            });

            Capi.Input.RegisterHotKey("csb2", "SandboxTest 2", GlKeys.Keypad2, HotkeyType.CharacterControls, false,
                true);
            Capi.Input.SetHotKeyHandler("csb2", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Saturation = 0.8f;
                Capi.ShowChatMessage($"Saturation: {_prog.Saturation}");
                return true;
            });

            //
            // NumPad 3
            //

            Capi.Input.RegisterHotKey("sb3", "SandboxTest 3", GlKeys.Keypad3);
            Capi.Input.SetHotKeyHandler("sb3", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Luminosity = GameMath.Clamp(_prog.Luminosity += 0.1f, 0, 100);
                Capi.ShowChatMessage($"Luminosity: {_prog.Luminosity}");
                return true;
            });

            Capi.Input.RegisterHotKey("ssb3", "SandboxTest 3", GlKeys.Keypad3, HotkeyType.CharacterControls, true);
            Capi.Input.SetHotKeyHandler("ssb3", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Luminosity = GameMath.Clamp(_prog.Luminosity -= 0.1f, 0, 100);
                Capi.ShowChatMessage($"Luminosity: {_prog.Luminosity}");
                return true;
            });

            Capi.Input.RegisterHotKey("csb3", "SandboxTest 3", GlKeys.Keypad3, HotkeyType.CharacterControls, false,
                true);
            Capi.Input.SetHotKeyHandler("csb3", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Luminosity = 0.5f;
                Capi.ShowChatMessage($"Luminosity: {_prog.Luminosity}");
                return true;
            });

            //
            // NumPad 4
            //

            Capi.Input.RegisterHotKey("sb4", "SandboxTest 4", GlKeys.Keypad4);
            Capi.Input.SetHotKeyHandler("sb4", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Intensity = GameMath.Clamp(_prog.Intensity += 0.1f, 0, 100);
                Capi.ShowChatMessage($"Intensity: {_prog.Intensity}");
                return true;
            });

            Capi.Input.RegisterHotKey("ssb4", "SandboxTest 4", GlKeys.Keypad4, HotkeyType.CharacterControls, true);
            Capi.Input.SetHotKeyHandler("ssb4", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Intensity = GameMath.Clamp(_prog.Intensity -= 0.1f, 0, 100);
                Capi.ShowChatMessage($"Intensity: {_prog.Intensity}");
                return true;
            });

            Capi.Input.RegisterHotKey("csb4", "SandboxTest 4", GlKeys.Keypad4, HotkeyType.CharacterControls, false,
                true);
            Capi.Input.SetHotKeyHandler("csb4", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Intensity = 0.0f;
                Capi.ShowChatMessage($"Intensity: {_prog.Intensity}");
                return true;
            });

            //
            // NumPad 5
            //

            Capi.Input.RegisterHotKey("sb5", "SandboxTest 5", GlKeys.Keypad5);
            Capi.Input.SetHotKeyHandler("sb5", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Speed = GameMath.Clamp(_prog.Speed += 0.1f, 0, 100);
                Capi.ShowChatMessage($"Speed: {_prog.Speed}");
                return true;
            });

            Capi.Input.RegisterHotKey("ssb5", "SandboxTest 5", GlKeys.Keypad5, HotkeyType.CharacterControls, true);
            Capi.Input.SetHotKeyHandler("ssb5", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Speed = GameMath.Clamp(_prog.Speed -= 0.1f, 0, 100);
                Capi.ShowChatMessage($"Speed: {_prog.Speed}");
                return true;
            });

            Capi.Input.RegisterHotKey("csb5", "SandboxTest 5", GlKeys.Keypad5, HotkeyType.CharacterControls, false,
                true);
            Capi.Input.SetHotKeyHandler("csb5", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Speed = 2.4f;
                Capi.ShowChatMessage($"Speed: {_prog.Speed}");
                return true;
            });

            //
            // NumPad 6
            //

            Capi.Input.RegisterHotKey("sb6", "SandboxTest 6", GlKeys.Keypad6);
            Capi.Input.SetHotKeyHandler("sb6", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Spread = GameMath.Clamp(_prog.Spread += 0.1f, 0.1f, 100f);
                Capi.ShowChatMessage($"Spread: {_prog.Spread}");
                return true;
            });

            Capi.Input.RegisterHotKey("ssb6", "SandboxTest 6", GlKeys.Keypad6, HotkeyType.CharacterControls, true);
            Capi.Input.SetHotKeyHandler("ssb6", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Spread = GameMath.Clamp(_prog.Spread -= 0.1f, 0.1f, 100f);
                Capi.ShowChatMessage($"Spread: {_prog.Spread}");
                return true;
            });

            Capi.Input.RegisterHotKey("csb6", "SandboxTest 6", GlKeys.Keypad6, HotkeyType.CharacterControls, false,
                true);
            Capi.Input.SetHotKeyHandler("csb6", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Spread = 3.8f;
                Capi.ShowChatMessage($"Spread: {_prog.Spread}");
                return true;
            });

            //
            // NumPad 7
            //

            Capi.Input.RegisterHotKey("sb7", "SandboxTest 7", GlKeys.Keypad7);
            Capi.Input.SetHotKeyHandler("sb7", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Brightness = GameMath.Clamp(_prog.Brightness -= 1, 0, 100);
                Capi.ShowChatMessage($"Brightness: {_prog.Brightness}");
                return true;
            });

            //
            // NumPad 8
            //

            Capi.Input.RegisterHotKey("sb8", "SandboxTest 8", GlKeys.Keypad8);
            Capi.Input.SetHotKeyHandler("sb8", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Brightness = 0;
                Capi.ShowChatMessage($"Brightness: {_prog.Brightness}");
                return true;
            });

            //
            // NumPad 9
            //

            Capi.Input.RegisterHotKey("sb9", "SandboxTest 9", GlKeys.Keypad9);
            Capi.Input.SetHotKeyHandler("sb9", _ =>
            {
                if (_overlayRenderer is not {Active: true})
                {
                    Capi.ShowChatMessage("Overlay Shader Not Active!");
                    return true;
                }

                _prog.Brightness = GameMath.Clamp(_prog.Brightness += 1, 0, 100);
                Capi.ShowChatMessage($"Brightness: {_prog.Brightness}");
                return true;
            });

            #endregion

            Capi.Input.RegisterHotKey("chaos-boom", "Boom!", GlKeys.ControlRight);
            Capi.Input.SetHotKeyHandler("chaos-boom", ExecuteCommand);
        }

        #endregion
    }
}