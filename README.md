# Cension Overview
Cension is a fine-tuned Transformer-based langauge model optimized for C# code. 
The project evaluates the benefits of domain-specific tuning over general-purpose models like GPT-4o. it includes:
1. a visual studio extension for real-time code prompting.
2. Evaluaiton pipeline accross correctness, efficiency and maintainability.
3. benchmarked against 30 LeetCode C# tasks.
4. benchmarked against GPT-4o

# Benchmark results
![Results](https://github.com/user-attachments/assets/cc0f9bfa-14fd-4e7c-a4b9-2a9d09ad99b8)


# Tech Stack
Python (for model training and finetuning)
React + Typescript for the UI
C# for building the extension and linking the UI to the extension framework

# Demo
![newlocationCension](https://github.com/user-attachments/assets/baec9d6d-730c-4c8c-848b-f8ac39e5feb1)
Cension in VS

![censionOnly](https://github.com/user-attachments/assets/f2badeac-0e7e-4703-99fd-61bba738e6b8)
Cension UI



# To run the cension:
1. Clone the repo in visual studio
2. Build the project
3. Locate the project in windows explorer
4. Go to bin -> Debug
5. Find CensionExtension and install it
6. In VS it will be foun under View -> Other Windows
(Requires administrative access to run propery)





