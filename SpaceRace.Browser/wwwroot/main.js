import { dotnet } from './_framework/dotnet.js'

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

const backgroundMusic = document.getElementById("background-music");
let musicStarting = false;

async function startBackgroundMusic(){
    if(musicStarting)
        return;
    musicStarting = true;

    await backgroundMusic.play();
    document.removeEventListener("click", startBackgroundMusic, true);
    musicStarting = false;
}

document.addEventListener("click", startBackgroundMusic, true);

const dotnetRuntime = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

const config = dotnetRuntime.getConfig();

await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
