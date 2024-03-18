using Nova;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine.Video;

public class MapSelectionManager : MonoBehaviour
{
    [SerializeField] ClipMask mapSelectionClipMask;

    public VideoPlayer previewPlayer;

    [SerializeField] AudioSource selectAudio;
    [SerializeField] AudioSource togglePositiveAudio;
    [SerializeField] AudioSource toggleNegativeAudio;

    [SerializeField] BackgroundManager background;
    [SerializeField] UIBlock2D backgroundUIBlock;
    [SerializeField] UIBlock coverCluster;
    [SerializeField] UIBlock2D[] covers;
    [SerializeField] UIBlock2D titleUIBlock;
    [SerializeField] TextBlock titleTextBlock;
    [SerializeField] TextBlock artist;
    [SerializeField] Texture2D[] difficulties;
    [SerializeField] UIBlock2D difficultiesUIBlock;
    [SerializeField] TextBlock difficultiesTextBlock;

    [SerializeField] GameObject connectionUI;
    [SerializeField] GameObject coachSelectionUI;

    public Playlists playlists;
    public SongDesc songDesc;

    public bool canInteract = false;
    public int selectedMap = 0;

    bool titleIsText = false;

    private async void Start()
    {
        background = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundManager>();
        previewPlayer = GameObject.Find("Video-Preview").GetComponent<VideoPlayer>();

        string path;
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            path = Application.dataPath;
        }
        else
        {
            path = Path.Combine(Directory.GetCurrentDirectory(), "Build/Just Dance Next_Data");
        }

        playlists = await Task.Run(async () => JsonConvert.DeserializeObject<Playlists>(await File.ReadAllTextAsync(Path.Combine(path, "Maps", "playlists.json"))));

        UpdateMenu();

        if (titleIsText)
        {
            LeanTween.value(0f, 1f, 0.2f).setOnUpdate((float value) =>
            {
                titleTextBlock.Color = new(1f, 1f, 1f, value);
            });
        }
        else
        {
            LeanTween.value(0f, 1f, 0.2f).setOnUpdate((float value) =>
            {
                titleUIBlock.Color = new(1f, 1f, 1f, value);
            });
        }

        await Task.Delay(50);

        background.StopMenuAudio();

        LeanTween.value(0f, 1f, 0.5f).setOnUpdate((float value) =>
        {
            mapSelectionClipMask.Tint = new(1f, 1f, 1f, value);
        }).setOnComplete(() =>
        {
            canInteract = true;
        });
    }

    private async void Update()
    {
        if (canInteract)
        {
            if (InputManager.Undo() && InputManager.source == InputManager.controller | InputManager.source == InputSource.Local)
            {
                canInteract = false;

                selectAudio.Play();

                LeanTween.value(1f, 0f, 0.5f).setOnUpdate((float value) =>
                {
                    mapSelectionClipMask.Tint = new(1f, 1f, 1f, value);
                }).setOnComplete(async () =>
                {
                    await Task.Delay(50);
                    Instantiate(connectionUI);
                    previewPlayer.Pause();
                    background.PlayMenuAudio();
                    Destroy(gameObject);
                });
            }
            else if (InputManager.Select() && InputManager.source == InputManager.controller | InputManager.source == InputSource.Local)
            {
                canInteract = false;

                selectAudio.Play();

                LeanTween.value(1f, 0f, 0.5f).setOnUpdate((float value) =>
                {
                    mapSelectionClipMask.Tint = new(1f, 1f, 1f, value);
                }).setOnComplete(async () =>
                {
                    await Task.Delay(50);
                    Instantiate(coachSelectionUI);
                });
            }
            else if (InputManager.Left() && InputManager.source == InputManager.controller | InputManager.source == InputSource.Local)
            {
                canInteract = false;

                LeanTween.cancelAll();

                togglePositiveAudio.Play();

                if (selectedMap - 1 < 0)
                {
                    selectedMap = playlists.playlistCluster[0].maps.Length - 1;
                }
                else
                {
                    selectedMap--;
                }

                if (titleIsText)
                {
                    titleTextBlock.Color = new(1f, 1f, 1f, 0f);
                }
                else
                {
                    titleUIBlock.Color = new(1f, 1f, 1f, 0f);
                }

                artist.Color = new(1f, 1f, 1f, 0f);
                difficultiesUIBlock.Color = new(1f, 1f, 1f, 0f);
                difficultiesTextBlock.Color = new(1f, 1f, 1f, 0f);
                backgroundUIBlock.Color = new(1f, 1f, 1f, 0f);

                UpdateMenu();

                coverCluster.Position.X = -378f;

                await Task.Delay(25);

                LeanTween.value(-378f, 0f, 0.2f).setOnUpdate((float value) =>
                {
                    coverCluster.Position.X = value;
                }).setEaseInOutCirc().setOnComplete(() =>
                {                    
                    if (titleIsText)
                    {
                        LeanTween.value(0f, 1f, 0.2f).setOnUpdate((float value) =>
                        {
                            titleTextBlock.Color = new(1f, 1f, 1f, value);
                        });
                    }
                    else
                    {
                        LeanTween.value(0f, 1f, 0.2f).setOnUpdate((float value) =>
                        {
                            titleUIBlock.Color = new(1f, 1f, 1f, value);
                        });
                    }

                    LeanTween.value(-7f, -2f, 0.2f).setOnUpdate((float value) =>
                    {
                        artist.Position.Y = value;
                    });
                    LeanTween.value(0f, 1f, 0.2f).setOnUpdate((float value) =>
                    {
                        artist.Color = new(1f, 1f, 1f, value);
                    });

                    LeanTween.value(0f, 1f, 0.2f).setOnUpdate((float value) =>
                    {
                        difficultiesUIBlock.Color = new(1f, 1f, 1f, value);
                    });
                    LeanTween.value(0f, 1f, 0.2f).setOnUpdate((float value) =>
                    {
                        difficultiesTextBlock.Color = new(1f, 1f, 1f, value);
                    });

                    LeanTween.value(0f, 0.25f, 0.2f).setOnUpdate((float value) =>
                    {
                        backgroundUIBlock.Color = new(1f, 1f, 1f, value);
                    });
                    LeanTween.value(0f, 0.25f, 0.2f).setOnUpdate((float value) =>
                    {
                        backgroundUIBlock.Color = new(1f, 1f, 1f, value);
                    });

                    canInteract = true;
                });
            }
            else if (InputManager.Right() && InputManager.source == InputManager.controller | InputManager.source == InputSource.Local)
            {
                canInteract = false;

                LeanTween.cancelAll();

                toggleNegativeAudio.Play();

                if (selectedMap + 1 >= playlists.playlistCluster[0].maps.Length)
                {
                    selectedMap = 0;
                }
                else
                {
                    selectedMap++;
                }

                if (titleIsText)
                {
                    LeanTween.value(1f, 0f, 0.2f).setOnUpdate((float value) =>
                    {
                        titleTextBlock.Color = new(1f, 1f, 1f, value);
                    }).setEaseInOutCirc();
                }
                else
                {
                    LeanTween.value(1f, 0f, 0.2f).setOnUpdate((float value) =>
                    {
                        titleUIBlock.Color = new(1f, 1f, 1f, value);
                    }).setEaseInOutCirc();
                }

                LeanTween.value(-2f, 3f, 0.2f).setOnUpdate((float value) =>
                {
                    artist.Position.Y = value;
                });
                LeanTween.value(1f, 0f, 0.2f).setOnUpdate((float value) =>
                {
                    artist.Color = new(1f, 1f, 1f, value);
                });

                LeanTween.value(1f, 0f, 0.2f).setOnUpdate((float value) =>
                {
                    difficultiesUIBlock.Color = new(1f, 1f, 1f, value);
                });
                LeanTween.value(1f, 0f, 0.2f).setOnUpdate((float value) =>
                {
                    difficultiesTextBlock.Color = new(1f, 1f, 1f, value);
                });

                LeanTween.value(0.25f, 0f, 0.2f).setOnUpdate((float value) =>
                {
                    backgroundUIBlock.Color = new(1f, 1f, 1f, value);
                });
                LeanTween.value(0.25f, 0f, 0.2f).setOnUpdate((float value) =>
                {
                    backgroundUIBlock.Color = new(1f, 1f, 1f, value);
                });

                LeanTween.value(0f, -378f, 0.2f).setOnUpdate((float value) =>
                {
                    coverCluster.Position.X = value;
                }).setEaseInOutCirc().setOnComplete(async () =>
                {
                    await Task.Delay(25);
                    UpdateMenu();
                    coverCluster.Position.X = 0f;
                    await Task.Delay(25);
                    if (titleIsText)
                    {
                        LeanTween.value(0f, 1f, 0.2f).setOnUpdate((float value) =>
                        {
                            titleTextBlock.Color = new(1f, 1f, 1f, value);
                        });
                    }
                    else
                    {
                        LeanTween.value(0f, 1f, 0.2f).setOnUpdate((float value) =>
                        {
                            titleUIBlock.Color = new(1f, 1f, 1f, value);
                        });
                    }

                    LeanTween.value(-7f, -2f, 0.2f).setOnUpdate((float value) =>
                    {
                        artist.Position.Y = value;
                    });
                    LeanTween.value(0f, 1f, 0.2f).setOnUpdate((float value) =>
                    {
                        artist.Color = new(1f, 1f, 1f, value);
                    });

                    LeanTween.value(0f, 1f, 0.2f).setOnUpdate((float value) =>
                    {
                        difficultiesUIBlock.Color = new(1f, 1f, 1f, value);
                    });
                    LeanTween.value(0f, 1f, 0.2f).setOnUpdate((float value) =>
                    {
                        difficultiesTextBlock.Color = new(1f, 1f, 1f, value);
                    });

                    LeanTween.value(0f, 0.25f, 0.2f).setOnUpdate((float value) =>
                    {
                        backgroundUIBlock.Color = new(1f, 1f, 1f, value);
                    });
                    LeanTween.value(0f, 0.25f, 0.2f).setOnUpdate((float value) =>
                    {
                        backgroundUIBlock.Color = new(1f, 1f, 1f, value);
                    });

                    canInteract = true;
                });
            }
        }
    }

    void UpdateMenu()
    {
        string path;
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            path = Application.dataPath;
        }
        else
        {
            path = Path.Combine(Directory.GetCurrentDirectory(), "Build/Just Dance Next_Data");
        }

        songDesc = JsonConvert.DeserializeObject<SongDesc>(File.ReadAllText(Path.Combine(path, "Maps", playlists.playlistCluster[0].maps[selectedMap].name, "songdesc.json")));

        for (int i = 0; i < covers.Length; i++)
        {
            if (i != 0)
            {
                int iSelectedMap;
                if (selectedMap + i >= playlists.playlistCluster[0].maps.Length)
                {
                    iSelectedMap = selectedMap + i - playlists.playlistCluster[0].maps.Length;
                }
                else
                {
                    iSelectedMap = selectedMap + i;
                }
                Texture2D texture = new(1024, 512, TextureFormat.RGBA32, false);
                texture.LoadImage(File.ReadAllBytes(Path.Combine(path, "Maps", playlists.playlistCluster[0].maps[iSelectedMap].name, "cover.png")));
                covers[i].SetImage(texture);
            }            
        }

        if (File.Exists(Path.Combine(path, "Maps", playlists.playlistCluster[0].maps[selectedMap].name, "title.png")))
        {
            Texture2D texture = new(1024, 512, TextureFormat.RGBA32, false);
            texture.LoadImage(File.ReadAllBytes(Path.Combine(path, "Maps", playlists.playlistCluster[0].maps[selectedMap].name, "title.png")));
            titleUIBlock.SetImage(texture);
            titleIsText = false;
        }
        else
        {
            titleTextBlock.Text = songDesc.title;
            titleIsText = true;
        }

        artist.Text = songDesc.artist;

        difficultiesUIBlock.SetImage(difficulties[songDesc.difficulty - 1]);
        switch (songDesc.difficulty)
        {
            case 1:
                difficultiesTextBlock.Text = "Difficulty: Easy";
                break;
            case 2:
                difficultiesTextBlock.Text = "Difficulty: Medium";
                break;
            case 3:
                difficultiesTextBlock.Text = "Difficulty: Hard";
                break;
            case 4:
                difficultiesTextBlock.Text = "Difficulty: Extreme";
                break;
        }

        previewPlayer.url = "file://" + Path.Combine(path, "Maps", playlists.playlistCluster[0].maps[selectedMap].name, "preview.mp4"); ;
        previewPlayer.Play();

        if (File.Exists(Path.Combine(path, "Maps", playlists.playlistCluster[0].maps[selectedMap].name, "bkg.png")))
        {
            Texture2D texture = new(2048, 1024, TextureFormat.RGBA32, false);
            texture.LoadImage(File.ReadAllBytes(Path.Combine(path, "Maps", playlists.playlistCluster[0].maps[selectedMap].name, "bkg.png")));
            backgroundUIBlock.SetImage(texture);
        }        

        Resources.UnloadUnusedAssets();
    }

    public void ReactivateFromCoachSelection()
    {
        LeanTween.value(0f, 1f, 0.5f).setOnUpdate((float value) =>
        {
            mapSelectionClipMask.Tint = new(1f, 1f, 1f, value);
        }).setOnComplete(() =>
        {
            canInteract = true;
        });
    }
}

public struct Playlists
{
    public PlaylistCluster[] playlistCluster;

    public struct PlaylistCluster
    {
        public string name;
        public Maps[] maps;
    }

    public struct Maps
    {
        public string name;
    }
}
