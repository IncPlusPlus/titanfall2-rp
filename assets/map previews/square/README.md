# Square cropped photos

Most of these use the autmatically determined square crop from [Croppola](https://croppola.com/).

Use something like [this script](https://unix.stackexchange.com/a/175137) for bulk renaming the files (because Croppola downloads all of them as "FILE_NAME Cropped.png"). Here's the script I used:

```bash
find . -type f -name '* Cropped.png' | while read FILE ; do
    newfile="$(echo ${FILE} |sed -r -e 's/(.*) Cropped(\.png)/\1\2/')" ;
    mv "${FILE}" "${newfile}" ;
done
```