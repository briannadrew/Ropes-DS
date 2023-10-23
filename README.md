# Ropes

This is a C# complex simulation of a Rope data structure, often used for string concatenation. Ropes are a binary tree structure where each node except the leaf nodes, ***contains the number of characters present to the left of that node***. Leaf nodes contain the actual string broken into substrings.

![Imgur](https://i.imgur.com/wlxUJse.jpeg)



## Installation

1. To use this program, ensure you have the latest edition of [Microsoft Visual Studio](https://visualstudio.microsoft.com/#vs-section) installed on your computer.

2. Download this repository as a zip file to your machine.

3. Unzip the files to your desired location.

4. Open the .sln or .csproj file with Visual Studio.

5. Press the green play button at the top! (will either say Start or Run).

   

## Usage

### Menu

These are the following options you can choose from in the program. Enter the corresponding digit for your desired option and press enter:

![Menu](https://i.imgur.com/QlJxn0F.png)



### 1 - Create New Rope

![](https://i.imgur.com/vKwRkjH.png)

![Img](https://i.imgur.com/IWqYgF6.png)

You can create a new rope in the slot 1 or 2. Simply enter which slot you would like to create your rope in, and then enter the string you would like to store.



### 2 - Insert Substring

![](https://i.imgur.com/Q43TtWu.png)

To insert a substring, you just select which rope you would like to insert your substring into, enter the substring you'd like to store, then enter the index where this substring should being (it will begin AFTER the index you enter for the original string. Remember, the first index is 0).



### 3 - Delete Substring

![](https://i.imgur.com/hhM6hXO.png)

To delete part of a string, simply select which rope you would like to delete from, and your starting and end indices. ***(Please note, the deletion example above was performed AFTER the rest of the functions below in this example).***



### 4 - Find Character

![](https://i.imgur.com/ItmGm2Q.png)

To find a character, simply select which rope to find a character within and give an index. The program will then tell you which character is being stored at that index in that rope.



### 5 - Find Substring

![](https://i.imgur.com/qVdn5Lr.png)

To find a substring from index a to b, simply enter which rope to find substring within, and then provide both indices. The program will output the substring found in that range.



### 6 - Split Rope

![](https://i.imgur.com/mAvAYsS.png)

To split a rope into two parts, select which rope you'd like to split and the desired index. ***(Please note, there is currently some unexpected behaviour on occasion within the Split function. An issue has been opened).***



### 7 - Concatenate Rope

![](https://i.imgur.com/B3sShYn.png)

Option 7 will concatenate ropes 1 and 2 into 1 rope (stored in rope 1).



### 8 - Print Rope

![](https://i.imgur.com/zzfYPOw.png)

To view a rope, simply select which rope you would like to be written to the console.



### 9 - Exit Program

![](https://i.imgur.com/AyTp9H8.png)

Finally, enter 9 to exit the program.
