{
  pkgs ? import <nixpkgs> { overlays = [ (import <bsuir-tex/nixpkgs>) ]; },
}:
with pkgs;
let
  dotnetPkg = with dotnetCorePackages; combinePackages [ sdk_8_0 ];
  dotnetDeps = [
    xorg.libX11
    xorg.libICE
    xorg.libSM
    fontconfig
  ];
in
mkShell rec {
  name = "SD-2";

  vscode-settings = writeText "settings.json" (
    builtins.toJSON {
      "dotnetAcquisitionExtension.sharedExistingDotnetPath" = DOTNET_ROOT;
    }
  );

  packages = [
    (texliveMedium.withPackages (_: with texlivePackages; [ bsuir-tex ]))
    dotnetPkg
    inkscape-with-extensions
    plantuml
    python312Packages.pygments
  ] ++ dotnetDeps;
  LD_LIBRARY_PATH = with pkgs; lib.makeLibraryPath dotnetDeps;
  DOTNET_ROOT = "${dotnetPkg}/dotnet";

  shellHook = ''
    mkdir .vscode &>/dev/null
    cp --force ${vscode-settings} .vscode/settings.json
  '';
}
