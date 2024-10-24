# Framework based on Large Language Models for creating customized virtual environment

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

FIRST PART

Large Language Models (LLMs) trained for code generation can be used to transform a natural language request for a virtual world into reality. The main objective of this framework is to utilize LLMs to generate custom virtual environments tailored to user requests. In recent years, AI models have achieved remarkable levels of reliability when responding to inquiries. Therefore, we intend to exploit these models and provide users with a framework that can overcome the problem of creating different environments for various situations with minimal effort. Our goal is to enable the construction of environments using natural language, making the framework accessible to individuals outside the realm of computer science. The possibilities for custom-designed VRs are virtually limitless. For instance, a physiotherapist could request a virtual environment that is tuned to the needs of a patient with an unusual injury. Students in a laboratory could inspect, analyse, and become familiar with materials or machines that they might not have access to because of budget constraints, safety concerns, or physical limitations. Furthermore, without the support of a framework like this, building the environment would be a boring and tedious task that must be repeated hundreds of times. 

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

SECOND PART

In the second part of this project, we have added several improvements; the first one is the possibility to use Gemini-Pro-1.0 as Large Language Model. The execution time of the script with Roslyn takes only few seconds after the script has been found by the LLM chosen.
The input request is now shorter and clearer.
It has been possible to use Gemini LLMs because of the implementation of a Pyhton Server that communicates directly with Gemini and send all the LLMs' responses to Unity, where they will be processed and analyzed by the system.
