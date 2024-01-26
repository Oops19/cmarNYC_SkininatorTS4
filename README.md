# TS4 Skininator - Version 2.6.1, 3/23/2023

## Additional Information
* This is a fork from MTS. 
* Original description by [CmarNYC](https://modthesims.info/member.php?u=3216596) who retired on earth and is now supporting the TS4 development in heaven.

### Requirements:
* TS4BodyUVtemplates.zip
* TS4FaceTextures.zip
* XmodsDataLibSource 2023-03-33
* t.b.d.

## Description

The TS4 skintone system has a lot of potential, and I've been working on a tool to make it easier to work with, similar to Skininator for TS3.

Definition of Terms: (These are the terms I'm using - they're not 'official'.)
TONE : A game file which defines a skintone. It links to the color images, the overlays, and contains various settings.
Skintone : The skin you click on in CAS, which is defined by a TONE file.
Body definition/details : A texture with shading, contouring, and details such as muscles and belly buttons. (Yes, I know there's a separate 'skin details' in CAS. Couldn't think of a better term.) There are separate textures for each age/gender/bodytype.
Skin color : A texture which applies color and some shading composited with the skin details. There is one skin color texture which is applied to all age/genders.
Overlay : A texture which is layered on top of the skin details and skin color. The ages and genders it applies to can be specified.
Burn Mask: (New with 6/2019 patch) An overlay giving burned skin pale areas around the eyes, neck, etc. Does not look good with dark skins.

Tool Overview:

Create/Edit Custom Skin Colors

    Cloning Tool : Clones game skintones and creates a package with the TONE files.
    Clone Package Editor : Open and modify cloned packages.
        TONE Manager : Add/Delete TONEs, modify flags defining the type and usage, modify the swatch color and various other settings, import and export the skin color images. Change property flags/tags, randomization tuning, and skintone panel (warm, neutral, cool, misc.) for selected skintones in a package.
        Overlay Manager : Add/Delete overlays, define which ages/genders each overlay applies to, import and export overlay images.
        Previewer : Preview skintones.

Create/Edit Default Replacement Skin Definition:

    Make New Default Replacement Package : Select which skin definition textures you want to replace and clone them to a new package.
    Edit/Test Default Replacement Package : Import/Export textures and preview the skin definitions.

Notes: (Please read these!!!)

    While the preview is reasonably accurate for game skintones, if you modify the overlaid color - especially if you increase the saturation level beyond 40 or so - you probably will not get the same color in-game. If you go over around 100-150 for the overlaid color saturation, you're likely to get weird effects in the game that don't show up in the previewer. EA uses some compositing method I don't understand and can't completely duplicate despite a lot of effort. CAS and the game are the only true test.
    If it seems like there's a million body skin definition textures, that's because THERE ARE. There's a separate texture for every age and every body type. For many purposes you're better off using an overlay. As far as I know there can be only one set of body skin definition textures, no non-defaults.
    Speaking of overlays, only one will be used for each age/gender.
    According to my tests, EA is now using LRLE textures for the skin color and for body skin definition/details; and an RLE2 texture for overlays and for burn masks. 

Moar notes:

    Randomization: Skins are chosen randomly based on the archetypes defined in their flags. The more archetypes a skin has, the more often it gets chosen. If it has no archetypes it will get chosen randomly only if there are no other skins that do have archetypes. The game skins for humans all have archetypes; the game skins for aliens don't have any. Edit: The latest version of the skin TONE resource includes tuning, which appears to determine for which sims a skin gets randomized. Example: skins with 'Human' tuning are randomized for human sims, with 'Vampire' tuning are randomized for vampire sims, etc. Skins with 'Fantasy' tuning don't seem to get randomized at all.
    The 'Occult' flag(s) determines if a skin shows up for humans, aliens, vampires, etc. Future occult types will probably have their own skins too. A skin must have an Occult / Human tag to show up for humans, an Occult / Alien tag to show up for aliens, an Occult / Mermaid tag for mermaids, etc.

I've also attached a set of templates showing how the body is mapped to the textures.

Face definition/details textures: There are about a billion of them and I haven't even tried to identify them and include them in the tool. I've uploaded two packages with all the face textures I could find, so anyone wanting to work with them can use that as a starting point.

Extract the attached zip, open the folder, and run TS4Skininator.exe. Please report any problems/suggestions/comments about the tool here. Please post problems and questions about creating specific content in the TS4 / Create / CAS Parts forum.

Tutorial: http://www.modthesims.info/showthread.php?t=568713

Additional Credits:
With thanks to Peter and Inge Jones, Kuree, and everyone else who's contributed to s4pi and s4pe, and to Snaitf for figuring out how to make non-default skintones.

Skininator uses the s4pi library for image and package handling. The latest s4pi source code can be found here: https://github.com/s4ptacle/Sims4Tools/tree/develop

The latest s4pe download which includes the image and package dlls used in Skininator can be found here: https://github.com/s4ptacle/Sims4Tools/releases

Updated 3/23/2023, version 2.6.1
- Added support for infants in the Overlay Manager tab.
- Add automatic conversion of default replacements to LRLE format.
- Replaced wrong version of DDS handler which was causing crashes when importing and exporting overlays.

Updated 3/22/2023, version 2.6
- Added support for infants.
- Added support for werewolves in default replacements and previews.
- Updated tags.

Updated 7/4/2022, version 2.5.1.0
- Updated tags to support werewolf occult.
- Updated LRLE compression to fix spotty or blotchy appearance in game.
NOTE: The werewolf form does not use skintones, it appears to use the pelt layer system for pets. A separate tool is needed to make custom werewolf skins.

Updated 1/18/2022, version 2.5.0.0
Changes are to the skin definition default replacement section.
-- Now lists and clones both skin definition layers for adults. (EA has main textures and breast/chest overlays for Teen, YA, and elder, and two identical textures for adults. In some cases when the adult main texture was modified the EA layer underneath would show through.)
-- The skin definition preview has been overhauled to more accurately show how skin definitions will look in-game, with options to blend the morph textures over the neutral texture. (The background fill option fills the face with neutral gray. In-game, face preset textures would be layered under the skin definition texture.)
-- When cloning default replacement skin definitions, you now have an option to automatically blend the overlays and underlayers into the main texture and create blank textures for the breast/chest overlays and the adult underlayer so they will not conflict in-game.

Updated 3/29/2021, version 2.4.0.0
-- Now correctly finds game textures and handles EA's empty textures. Cloned packages should now open without an error.
-- Automatically converts old versions of the TONE and outdated textures to new versions and formats. In many cases packages will be smaller because duplicate textures are eliminated.
-- Overhauled texture import/export, overlay handling, and cloning of CC packages.
-- The option to save for Legacy Edition will convert the TONEs and textures to LE-compatible formats. Packages may be bigger because of duplicate textures.
There are a lot of changes under the hood so this is pretty much a beta.

Updated 1/11/2021, version 2.3.0.0
-- Added support for the LRLE format. Textures from old packages will be updated to LRLE on import. LRLE can be used with HQ textures but this needs more testing. I haven't tested in general as much as I'd like so please report problems. Also note that LRLE textures in many/most cases will be bigger than the old RLE textures; this is due to LRLE being lossless and keeping full detail.

Updated 12/20/2020, version 2.2.2.0
-- Corrected a layout problem causing the skin definitions package editor's Save buttons to drop off the window.

Updated 12/15/2020, version 2.2.1.0
-- Corrected bug causing the CAS skin panel not to save correctly when changed for individual skin colors. Users of 2.2 should update.

Updated 12/13/2020, version 2.2.0.0
-- Supports expanded skintones introduced in the Dec. 2020 patch. (You must choose to update to latest version for the skintones to work in patched games.)
-- Added ability to change skintone panel choice for selected skintones.
-- Added option to save a package for Legacy Edition. (I haven't tested this personally so please report errors.)
-- The preview needs tweaking, for a future version.
-- The EA toddler skin definition textures were converted to LRLE and are not usable, to be added in a future version.

Updated 6/23/2020, version 2.1.0.0
-- Now supports HQ textures and overlays.
-- Now runs in 64-bit.
-- Will correct for missing mipmaps.

Notes: I had to modify some of the DDS file code because HQ textures were causing out of memory errors. It seems to be working reliably now but please report problems. Since this is kind of beta-ish I'm leaving the previous version up for now.

Updated Property Tags, 12/15/2019
The property tags definition file has been updated to support Witches. Download "TS4 PropertyTags 12-2019.zip" and extract to the folder where your TS4Skininator files are located, overwriting the old version. Then run Skininator as usual. If you're using version 2.1.0.0 you do NOT need this file.

Updated 7/9/2019, version 2.0.0.0
-- Many changes to support tanning and burning.
----- There are now three sets of skin textures for the normal, tanned, and burned states.
----- Burn masks are optional but can be imported for all three sets. EA only uses them for the burned state in lighter skins. They appear not to be used at all in the normal skin state.
----- I've removed the ability to import separate CAS textures and game textures since there are so many textures to deal with now and I don't know of any skintone creator actually using that feature.
----- The new burn mask multiplier magnifies the effect of a burn mask.
-- Changes to property tags and tuning can now be applied from the main skintone editor tab.
-- Property tags updated.
-- This is sort of an interim version - I probably missed some bugs and the preview needs work.

Updated 8/6/2018, version 1.12.0.0
Added ability to import/export PNG image files.
'Manage Flags for All' is now 'Manage Properties for Selected / Update Version':
-- Property tags can now be changed for selected skintones instead of all
-- All skintones in the package can be updated to the latest version, enabling the Randomization Tuning. The conversion code will make an educated guess of which tuning to select based on property tags.
Updated properties/tags.

6/18/2018
Updated the body UV templates to include toddlers.

Updated 11/6/2017, version 1.10.0.0
Bugfix for program exit when unable to find game package files.
Some fiddling with preview skin color which probably won't make any visible difference.
Updated properties/tags.

Updated 6/27/2017, version 1.9.0.0
Bugfix for errors when editing old format skintones. Again I didn't test adequately, sigh.
Added proper scaling for different screen resolutions, so hopefully this will help with appearance and resizing issues.
Updated properties/tags.

Updated 3/22/2017, version 1.8.0.0
Updated for new version of TONE file. I'm not sure when the new version happened, but I probably didn't test adequately after the February patch, so apologies.
The new TONE format links to a data resource that's identified as skintone tuning. I don't see that it makes any difference but have included changing it as an option in Skininator. (Edit - the tuning seems to determine which if any sim type can get the skintone randomly.)

Updated 2/9/2017, version 1.7.0.0
Sorry this took so long - RL travel, family commitments, and illness.
Added full support for toddlers.
Will now recognize the Occult / Vampire tag so skins can be enabled for vampires.

Updated 11/30/16, version 1.6.0.0
New feature: A new "Manage Flags for All" tab has been added to the Clone Package Editor in the "Create/Edit Custom Skin Colors" tab. This new function allows you to add or remove property flags/tags from all the skintones in a package.

Updated 9/3/16, version 1.5.0.0
Updated for compatibility with male chest overlays, which seem to have been implemented in a patch sometime in July or August.
Bugfix: The program will now export textures with mipmaps intact.
New feature: In the "Create/Edit Default Replacement Skin Details" tab there's now an added "Convert Outdated Package to New Instance IDs" tab. This will convert a pre-June patch default replacement skin to work with the current game.

Updated 6/28/16, version 1.4.0.0
This update is to make the program compatible with the June game patch, which made some changes to the skin details textures:
-- The male and female adult instance IDs have changed and are updated in the program.
-- The new female breast overlay images are now included.
-- Note that the adult females do NOT have breast overlays. Teen, YA, and Elders do.
-- Note that the old male adult textures for the muscle/bony/fat/thin morphs are being overlaid by the new textures. They show through faintly but I dropped them from the program since there's already a million billion textures to deal with and I don't think they're visible enough to matter.

Updated 1/31/16, version 1.3.0.0
Fixed bug causing new content clones to replace game skin textures.
Moved property tags to external files which can be easily updated without making a new version of the program.

Updated 12/12/15, version 1.2.0.0
Fixed overlay manager to update the overlay list when changes are committed. Added selection of ages teen to elder to previewer. 