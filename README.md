DiffPatcher
=============================================================================

Simple patch creator and applier aimed at applications and games.

How it works
-----------------------------------------------------------------------------

The `DiffPatchCreator` allows the comparision of two folders, creating a
single patch file (zip archive) to patch the old version to be equal to
the new version, using delta compression, to only patch what is necessary,
and not replace entire files.

`DiffPatcher` in turn downloads these patch files and applies them
to the files in the folder it is in.

Both are currently using xdelta3.exe, for diffing and applying patches
to changed files. Executables for Linux and Mac can be found in the
Libs folder, but the source code has to be changed slightly for them
to be used.

Versioning
-----------------------------------------------------------------------------

Every patch gets assigned a version, or revision number that it uses
to determine if there are new patches available. If there's a patch
with version 3, and its local version is 2, it will download the new
patch.

It doesn't matter if there are versions missing in between, it will simply
download them in order.

If the local version is 0, or the `version` file, where the local version
is stored, is deleted, the patcher downloads everything over. Due to this
behavior it's a good idea to make the first "patch" a diff between an
empty folder and the full application, as that will allow users to
redownload everything by deleting the version file, getting a clean slate.

Patcher configuration
-----------------------------------------------------------------------------

### `patch_uri`

URI to the folder that contains the patch list and the patches listed in
said list.

### `patch_list`

List of patch files for the patcher to download and apply in the following
format:

```
<version> <file_name>
```
Example:
```
1  1_full_application.zip
2  2_some_bug_fix.zip
3  3_new_feature.zip
```

Ignores empty lines and lines prefixed with `!`, `;`, `#`, `//`, or `--`.

### `exe`

Name of the executable file to start when the Start button is clicked
after patching. If this is empty, the Start button will be disabled.

### `arguments`

Arguments to pass to the exe that is started when the Start button
is clicked.

Licenses
-----------------------------------------------------------------------------

DiffPatcher is licensed under GPLv3. It makes use of xdelta3 and DotNetZip.
The respective licenses can be found in LICENSE.md and LICENSE-3RD-PARTY.md.
