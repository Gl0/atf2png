# atf2png

Managed library, able to read ATF (Adobe Texture Format) and dissect it into different frames using xml texture atlas.
Such combination (ATF + xml) is commonly used in Flash/Haxe games that utilize Starling framework.

Bundled with simple command line application that converts .atf file into .png or .atf + .xml into a bunch of .png files containing all frames.

Currently only ATF without lzma or jpeg compression supported.
Thanks to MonoGame project for DXT decompression code.