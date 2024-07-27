# SOPtoWorkWithGit

## 冷知识：SOP是什么？

> 作业标准书，英文全称“**Standard Operating Procedure**”，简称SOP，英文直译是“标准操作流程”，国内习惯称之为作业标准书，又称作业指导书。作业标准书是把现场所有的工作制定出一套流程每个人按部就班的按照流程来执行。在实际的生产中在不断的对SOP进行修改和完善，以提高操作的规范性和高效性。

为了防止大家在本次数据库课程设计中在使用git进行版本控制时出现各种各样的问题，本文会对git——这个对大家以后的一生工作都会打交道的工具进行一般标准工作流程的梳理。

本教程的github链接：https://github.com/IamLCB/SOP-to-WorkWithGit，本文也会在该仓库中进行操作示范。

## 0x00.获得远程仓库-gitgit clone

很显然，工作开始时，你需要得到初始的仓库内容（本文不涉及创建仓库的部分，默认从co-worker的视角进行）。

在你的任一目录下（一般为了不出bug，建议使用全英文目录）运行 ***git clone*** 命令：

`git clone <your-repo-url>`

<img src="..\..\Images\SOPtoWorkWithGit\00-clone.png" alt="image-20240511174014361" style="zoom:80%;" />

这样，你就能在你的目录下得到一个新的文件夹，在本文中，为`SOP-to-WorkWithGit`

## 0x01.正式开始工作前的准备-git checkout与git pull

打开上一步获得的`SOP-to-WorkWithGit`，在空白处 **右键-显示更多选项（for win11）-Open Git Bash Here** 即能打开一个新的git窗口，此时会显示如下信息：

<img src="..\..\Images\SOPtoWorkWithGit\01-1-gitbash.png" alt="image-20240511175055020" style="zoom:80%;" />

如果你不想被复杂的**`merge conflict`**所苦恼，我们先要对有可能被别的用户所更改的远端仓库内容做更新，此时，使用 ***git pull*** 命令，只需要在命令行中输入：

`git pull`

<img src="..\..\Images\SOPtoWorkWithGit\01-2-pull.png" alt="image-20240511175055020" style="zoom:80%;" />

在输出的指令中，会提示**`哪些分支发生了修改与更新`**以及**`更新的文件`**。

当然，更常规的情况是我们会使用多分支来进行合理的版本控制，那么在此时我们会先进行***切换分支***的操作，

使用指令：

`git checkout <your-branch-name>`

<img src="..\..\Images\SOPtoWorkWithGit\01-3-checkout.png" alt="image-20240511175055020" style="zoom:80%;" />

如果你在第一次打开git bash的时候就使用过 ***git pull*** 指令，此时会提示:

`Your branch is behind 'origin/<your-branch-name>' by x commit, and can be fast-forwarded.`

其中，最重要的是 ***can be fast-forwarded***，这个意味着可以快速合并，那么我们此时再使用 ***git pull*** 指令就可以快速保持本地内容与远程仓库一致了，当然，也有时候会出现 ***cannot*** 的情况，我们会在文章最后讲解处理 ***merge conflict*** 的问题。

#### 此时，你就可以正常的开始你的开发工作了！

## 0x02 开发完成后提交工作-status, add, commit, push

假设我们现在在 ***`dev`*** 分支下完成了开发工作，那么可以使用 ***git status*** 指令查看文件更新状况，在bash内使用

`git status`

<img src="..\..\Images\SOPtoWorkWithGit\02-1-status.png" alt="image-20240511175055020" style="zoom:80%;" />

通过结果可知，分为两类文件：

**1. Changes not staged for commit：**意味着更新的文件，也就是说原来就有，发生了更改

**2. Untracked files：**意味着新增的文件，以前没有，现在有了

那么接下来，我们需要把这些更改加入 ***提交暂存区*** ，使用 ***git add*** 命令：

`git add <your-file-name>`（当然，***your-file-name*** 可以使用一个 **.** 来快速添加所有文件）

<img src="..\..\Images\SOPtoWorkWithGit\02-2-add.png" alt="image-20240511175055020" style="zoom:80%;" />

注意，这里我们使用了 **`git add .`** 来快速添加所有文件，然后我们再使用 **`git status`** 命令查看状态，可以发现文件都变成了**绿色**，`Changes to be committed`，可以进行下一步的提交操作了。

下一步，我们将 ***提交暂存区***  正式提交，使用 ***git commit*** 命令：

`git commit [-m 'your-commit-message']` （当然，git commit有很多参数，本文在此处只展示最常见的方法）

`-m`参数可以快速添加*commit message*，***注意，一定要写commit message，不然此次commit等于无效***

当然，`-m`参数适合用于一句话的*commit message*，如果你想写一个详细的*commit log*，那么你可以不添加`-m`参数，这样在提交后会进入vim编辑器，可以撰写更详细的*commit log*

> 冷笑话：如何让一个计算机新手写出一本 *Harry Potter* ，在不给出任何提示的情况下让他退出vim编辑器。

<img src="..\..\Images\SOPtoWorkWithGit\02-3-commit.png" alt="image-20240511175055020" style="zoom:80%;" />

进入vim编辑器后，点击键盘上的 **i** 键进入编辑状态，此时左下角会显示 ***-- INSERT --*** ，可以使用**方向键**来移动光标，其中：**#** 开头的类似注释，不会显示在最终的文字中，**~** 为空行标记。

<img src="..\..\Images\SOPtoWorkWithGit\02-4-vim.png" alt="image-20240511175055020" style="zoom:80%;" />

撰写完毕`commit log/message`之后，可以按下 **ESC** 键退出编辑状态，此时左下角的 ***-- INSERT --*** 会消失，此时，输入 **`:wq`** + **回车** 即可退出vim编辑器。（退出编辑状态后，直接输入**英文冒号**，左下角就会出现输入的内容）

> 虽然大家需要有一定的英文读写能力，但是还是建议大家写中文的log/message，方便大家阅读。

<img src="..\..\Images\SOPtoWorkWithGit\02-5-exitvim.png" alt="image-20240511175055020" style="zoom:80%;" />

退出vim后，就会自动提交commit。

现在我们已经成功的把我们的工作提交到了**本地仓库**，那么接下来我们就需要将本地的工作**提交到远端仓库**，使用 ***git push*** ：

`git push` 

<img src="..\..\Images\SOPtoWorkWithGit\02-6-push.png" alt="image-20240511175055020" style="zoom:80%;" />

如果没有什么大问题，一般就会成功push，此时你已经把你的工作成功提交到了远端仓库，也就完成了一次标准的工作流程。***当然，对于团队管理者，还会涉及到合并分支、处理冲突的问题，我们会在下一节当中解释。***

## 0x03 团队管理者-merge&merge conflict

那么现在假设，我们手下的员工完成了工作的提交，他们提交到了 **`dev`** 分支下，现在，我需要把dev分支与main合并，也就是说 **把dev分支下更新的内容提交到main分支**（由于笔者的默认设置，本教程中主分支是master分支），那么我们就需要使用 ***git merge***，此处我们展示一种最简单的方式：

```
1. git checkout master
2. git merge <your-branch-name>
```

<img src="..\..\Images\SOPtoWorkWithGit\03-1-conflict.png" alt="image-20240511175055020" style="zoom:80%;" />

***非常不巧，***我们此处出现了 ***CONFLICT*** 的情况，也就是说，正如图中展示的，***git status*** 告诉我们，对于 `README.md` 这个文件，master分支和dev分支都做了修改，而且修改的内容不一样（一般也不会运气好到两个人修改的完全一样），现在我们就要处理 ***merge conflict*** 的情况，我们可以使用 ***VSCode*** 打开这个文件：

<img src="..\..\Images\SOPtoWorkWithGit\03-2-vscode.png" alt="image-20240511175055020" style="zoom:80%;" />

打开之后，什么都别管，直接点击进入 ***在合并编辑器中解析***，就能进入到非常方便的**图形化界面**

<img src="..\..\Images\SOPtoWorkWithGit\03-3-merging.png" alt="image-20240511175055020" style="zoom:80%;" />

（由于一些编码问题，所有的中文都变成了乱码，简单来说，DEV分支内的README.md比master分支中，结尾多了一行空行，一行*This is MY DEV WORK* ）

那么，如图所示，你可以选择**接受 传入**、**接受 当前** 或是 **接受 组合**，来快速解决合并问题，当然，没有一种方案可以解决所有问题，在实际情况中，更可能的是你甚至需要手动修改代码文件来处理 ***merge conflict***

<img src="..\..\Images\SOPtoWorkWithGit\03-4-merge.png" alt="image-20240511175055020" style="zoom:80%;" />

在解决 ***merge conflict*** 后，再查看status，就能看见所有的都变绿了，那么接下来直接使用

`git commit` 来提交你的merge操作并使用 `git push` 推送至远端仓库。

<img src="..\..\Images\SOPtoWorkWithGit\03-5-mergelog.png" alt="image-20240511175055020" style="zoom:80%;" />

<img src="..\..\Images\SOPtoWorkWithGit\03-6-mergesuccess.png" alt="image-20240511175055020" style="zoom:80%;" />

> 建议在merge操作的commit log中点出【合并】的操作，便于团队合作者了解主分支的版本。

**注：如果你在合并时没有遇到错误，使用 git merge 之后会自动进入vim编辑器，撰写完成commit log之后就自动完成了一次merge操作。**

# 0x04 结语

以上，便是最基础的git使用教程，我们在文中提到的每一个指令都有不同的参数，比如commit和push的 

`--dry-run` 参数可以在不发生修改的情况下测试操作后的情况，因此还需要大家今后进一步的学习。

此外，如果你非常不喜欢使用命令行界面，你也可以使用 ***Github Desktop*** 或是简单的 ***VSCode*** ，前者是最正统的图形化git管理器，而后者自带了一些更人性化的按钮与工具，快速进行git的相关操作，大家可以根据自己的实际情况选择软件使用。

最后，祝大家都能成为git高手。



> *Written by IamNotLCB, 2024-5-14*