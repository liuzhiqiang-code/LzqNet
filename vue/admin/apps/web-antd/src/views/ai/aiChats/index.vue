<script setup lang="ts">
import { ref, nextTick, watch, onMounted, onUnmounted } from 'vue'
import { useAppConfig } from '@vben/hooks';
import { useAccessStore } from '@vben/stores';
import MarkdownIt from 'markdown-it';

const { apiURL } = useAppConfig(import.meta.env, import.meta.env.PROD);
const accessStore = useAccessStore();

const md = new MarkdownIt({
  html: false,        // 禁用 HTML 标签，防止 XSS
  linkify: true,      // 将 URL 转换为链接
  typographer: true,  // 启用语义替换（如引号美化）
});

interface Message {
  id: number
  role: 'user' | 'ai'
  content: string
}

const currentTitle = ref('新对话')

const models = ref(['DeepSeekChat', 'DeepSeekReasoner'])

const messages = ref<Message[]>([])

type historyItem = {
  id: number,
  title: string
}
const history = ref<historyItem[]>([])

const inputText = ref('')
const selectedModel = ref<string>()
selectedModel.value = models.value[0]

const isModelOpen = ref(false)
const scrollContainer = ref<HTMLElement | null>(null)
const textareaRef = ref<HTMLTextAreaElement | null>(null)

const handleOutsideClick = (e: MouseEvent) => {
  const target = e.target as HTMLElement
  if (!target.closest('.model-selector')) {
    isModelOpen.value = false
  }
}

onMounted(() => window.addEventListener('click', handleOutsideClick))
onUnmounted(() => window.removeEventListener('click', handleOutsideClick))

const adjustHeight = () => {
  const textarea = textareaRef.value
  if (!textarea) return
  textarea.style.height = 'auto'
  textarea.style.height = `${Math.min(textarea.scrollHeight, 180)}px`
}

watch(inputText, () => { nextTick(() => adjustHeight()) })

const scrollToBottom = async () => {
  await nextTick()
  if (scrollContainer.value) {
    scrollContainer.value.scrollTo({
      top: scrollContainer.value.scrollHeight,
      behavior: 'smooth'
    })
  }
}

watch(messages, () => scrollToBottom(), { deep: true })

const sendMessage = async () => {
  const prompt = inputText.value.trim()
  if (!prompt) return

  // 1. 添加用户消息
  const userMsgId = Date.now()
  messages.value.push({ id: userMsgId, role: 'user', content: prompt })
  inputText.value = ''
  
  // 重置输入框高度
  nextTick(() => { if (textareaRef.value) textareaRef.value.style.height = 'auto' })

  // 2. 准备 AI 占位消息
  const aiMsgId = userMsgId + 1
  messages.value.push({ id: aiMsgId, role: 'ai', content: '' })
  
  try {
    const response = await fetch(`${apiURL}/aiChats/completion`, {
      method: 'POST',
      headers: { 
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${ accessStore.accessToken as string }`
      },
      body: JSON.stringify({
        Prompt: prompt,
        ChatClient : selectedModel.value,
        AIChatsId: 0 // 如果是新对话传0，或者根据实际业务传入当前对话ID
      })
    })

    if (!response.body) return

    const reader = response.body.getReader()
    const decoder = new TextDecoder()
    
    // 3. 循环读取流
    while (true) {
      const { value, done } = await reader.read()
      if (done) break

      const chunk = decoder.decode(value, { stream: true })
      
      // 解析后端自定义的 SSE 格式
      // 格式示例：message: {"v": "你好"}\n\ntitle: {"content": "新标题"}\n\n
      const lines = chunk.split('\n\n')
      
      for (const line of lines) {
        if (!line.trim()) continue

        // 处理流式文本消息
        if (line.startsWith('message:')) {
          const jsonStr = line.replace('message:', '').trim()
          try {
            const data = JSON.parse(jsonStr)
            // 后端 chunk 结构是 { v: "内容" } 或包含 BATCH 逻辑
            if (data.v && typeof data.v === 'string') {
              const aiMsg = messages.value.find(m => m.id === aiMsgId)
              if (aiMsg) aiMsg.content += data.v
            }
          } catch (e) {
            console.error('解析消息失败', e)
          }
        }

        // 处理标题更新
        if (line.startsWith('title:')) {
          const jsonStr = line.replace('title:', '').trim()
          try {
            const data = JSON.parse(jsonStr)
            if (data.content) {
              currentTitle.value = data.content
              // 同步更新侧边栏历史记录中的标题
              const histItem = history.value.find(h => h.title == data.content) // 假设匹配当前ID
              if (histItem) {
                histItem.title = data.content
              }
              else{
                history.value.push({
                  id : 0,
                  title:data.content
                })
              }
            }
          } catch (e) { }
        }

        // 处理关闭信号或其他自定义字段 (close:, update_session:)
        if (line.startsWith('close:')) {
          console.log('AI 响应结束')
        }
      }
    }
  } catch (error) {
    console.error('请求出错:', error)
    const aiMsg = messages.value.find(m => m.id === aiMsgId)
    if (aiMsg) aiMsg.content = '抱歉，服务遇到了一点问题，请稍后再试。'
  }
}
</script>

<template>
  <div class="flex h-screen w-full bg-[#050505] text-[#e5e7eb] overflow-hidden antialiased font-sans">

    <aside class="w-64 bg-[#0d0d0d] hidden lg:flex flex-col border-r border-[#1f1f1f] flex-shrink-0">
      <div class="p-6">
        <button @click="messages = []"
          class="w-full flex items-center justify-center gap-2 px-4 py-3 bg-[#161616] hover:bg-[#1a1a1a] text-gray-200 rounded-xl transition-all border border-[#303030] hover:border-[#0960bd] shadow-sm active:scale-[0.98] text-[13px] font-medium">
          <span class="text-xl text-[#0960bd]">+</span>
          <span>新建对话</span>
        </button>
      </div>

      <div class="flex-1 overflow-y-auto custom-scrollbar p-3">
        <div class="text-[10px] text-gray-600 mb-4 px-4 uppercase font-bold tracking-widest">历史记录</div>
        <div class="space-y-1">
          <div v-for="item in history" :key="item.id" @click="currentTitle = item.title"
            :class="['px-4 py-3 text-[13px] truncate cursor-pointer transition-all rounded-xl',
              currentTitle === item.title ? 'bg-[#0960bd]/10 text-[#0960bd]' : 'text-gray-500 hover:bg-[#161616] hover:text-gray-300']">
            {{ item.title }}
          </div>
        </div>
      </div>

      <div class="p-4 border-t border-[#1f1f1f]">
        <button
          class="w-full flex items-center gap-3 px-4 py-3 text-gray-500 hover:text-[#0960bd] hover:bg-[#0960bd]/5 rounded-xl transition-all text-[13px]">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
          </svg>
          <span>设置智能体</span>
        </button>
      </div>
    </aside>

    <main class="flex-1 flex flex-col min-w-0 h-full overflow-hidden">

      <header class="h-14 flex-shrink-0 flex items-center justify-center px-8 border-b border-white/[0.02]">
        <div class="flex items-center gap-3 bg-[#111]/60 backdrop-blur-xl px-5 py-1.5 rounded-full border border-white/5 shadow-xl">
          <div class="w-1.5 h-1.5 bg-[#52c41a] rounded-full animate-pulse shadow-[0_0_8px_#52c41a]"></div>
          <h1 class="text-[12px] font-medium text-gray-300 tracking-wide max-w-[260px] truncate">
            {{ currentTitle }}
          </h1>
          <div class="h-3 w-[1px] bg-gray-800 mx-1"></div>
          <span class="text-[9px] text-gray-600 uppercase font-black">Online</span>
        </div>
      </header>

      <section ref="scrollContainer" class="flex-1 overflow-y-auto custom-scrollbar scroll-smooth">
        <div class="max-w-4xl mx-auto py-8 px-6">
          <div v-for="msg in messages" :key="msg.id" :class="['flex w-full mb-8 animate-fade-in',
            msg.role === 'user' ? 'justify-end' : 'justify-start']">

            <div :class="['flex max-w-[85%] items-start gap-4',
              msg.role === 'user' ? 'flex-row-reverse' : 'flex-row']">

              <div v-if="msg.role === 'ai'" class="w-9 h-9 rounded-full flex-shrink-0 flex items-center justify-center 
                bg-[#161616] border-2 border-[#0960bd] shadow-[inset_0_0_8px_rgba(9,96,189,0.2)]">
                <svg class="w-5 h-5 text-[#0960bd]" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                  stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M12 8V4H8"></path>
                  <rect width="16" height="12" x="4" y="8" rx="2"></rect>
                  <circle cx="9" cy="13" r="1"></circle>
                  <circle cx="15" cy="13" r="1"></circle>
                </svg>
              </div>

              <div :class="['flex flex-col min-w-0', msg.role === 'user' ? 'items-end' : 'items-start']">
                <div :class="[
                  'text-[14px] leading-[1.7] py-3 px-5 transition-all relative',
                  msg.role === 'user'
                    ? 'bg-[#0960bd] text-white rounded-2xl rounded-tr-none shadow-lg shadow-[#0960bd]/10'
                    : 'bg-[#1a1a1a] text-[#cbd5e1] rounded-2xl rounded-tl-none border border-[#262626]'
                ]">
                  <div 
                    v-if="msg.role === 'ai'" 
                    class="markdown-body" 
                    v-html="msg.content ? md.render(msg.content) : '<span class=\'animate-pulse\'>...</span>'"
                  ></div>

                  <div v-else class="whitespace-pre-wrap break-words">
                    {{ msg.content }}
                  </div>
                </div>

                <div v-if="msg.role === 'ai'" class="mt-2 px-1">
                  <span class="text-[9px] text-gray-600 font-bold uppercase tracking-widest">{{ selectedModel }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <footer class="flex-shrink-0 p-6 bg-gradient-to-t from-[#050505] via-[#050505] to-transparent">
        <div class="max-w-4xl mx-auto">
          <div class="bg-[#111] rounded-2xl border border-[#262626] focus-within:border-[#0960bd]/50 transition-all shadow-2xl overflow-hidden">
            
            <textarea ref="textareaRef" v-model="inputText" @keydown.enter.exact.prevent="sendMessage"
              placeholder="发送指令..."
              class="w-full bg-transparent border-none focus:ring-0 px-6 py-4 resize-none text-[14px] text-gray-200 placeholder-gray-600 custom-scrollbar-textarea"
              style="min-height: 56px; max-height: 160px;"></textarea>

            <div class="flex items-center justify-between px-4 py-3 border-t border-white/[0.03] bg-white/[0.01]">
              <div class="flex items-center gap-2 text-gray-500 text-[11px] font-medium px-2">
                可按 Enter 发送
              </div>

              <div class="flex items-center gap-3">
                <div class="relative model-selector">
                  <button @click.stop="isModelOpen = !isModelOpen"
                    class="flex items-center gap-2 bg-[#1a1a1a] border border-[#333] text-[10px] text-gray-400 font-bold px-3 py-1.5 rounded-lg hover:bg-[#222]">
                    <span>{{ selectedModel }}</span>
                    <svg :class="['w-3 h-3 transition-transform', isModelOpen ? 'rotate-180' : '']" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path d="M19 9l-7 7-7-7" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" />
                    </svg>
                  </button>

                  <transition name="vben-pop">
                    <div v-if="isModelOpen" class="absolute bottom-full mb-2 right-0 w-44 bg-[#1a1a1a] border border-[#333] rounded-xl shadow-2xl p-1.5 z-[50]">
                      <div v-for="model in models" :key="model"
                        @click="selectedModel = model; isModelOpen = false"
                        :class="['px-3 py-2 text-[11px] font-medium rounded-lg cursor-pointer transition-all mb-0.5',
                          selectedModel === model ? 'bg-[#0960bd]/20 text-[#0960bd]' : 'text-gray-400 hover:bg-white/5']">
                        {{ model }}
                      </div>
                    </div>
                  </transition>
                </div>

                <button @click="sendMessage" :disabled="!inputText.trim()"
                  :class="['px-5 py-1.5 rounded-xl transition-all font-bold text-[12px] flex items-center gap-2',
                    inputText.trim() ? 'bg-[#0960bd] text-white shadow-lg shadow-[#0960bd]/20' : 'bg-[#1a1a1a] text-gray-600 cursor-not-allowed']">
                  <span>发送</span>
                  <svg class="w-3.5 h-3.5" fill="currentColor" viewBox="0 0 24 24">
                    <path d="M2.01 21L23 12 2.01 3 2 10l15 2-15 2z" />
                  </svg>
                </button>
              </div>
            </div>
          </div>
          <div class="mt-3 text-center">
            <p class="text-[10px] text-gray-700">AI 可能会产生错误，请核实重要信息。</p>
          </div>
        </div>
      </footer>
    </main>
  </div>
</template>

<style scoped>
/* 确保容器高度填满视口 */
.h-screen {
  height: 90vh;
}

/* 滚动条美化 */
.custom-scrollbar::-webkit-scrollbar {
  width: 5px;
}

.custom-scrollbar::-webkit-scrollbar-thumb {
  background: #262626;
  border-radius: 10px;
}

.custom-scrollbar-textarea::-webkit-scrollbar {
  display: none;
}

/* 动画 */
.animate-fade-in {
  animation: fadeIn 0.4s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

.vben-pop-enter-active,
.vben-pop-leave-active {
  transition: all 0.2s ease;
}

.vben-pop-enter-from,
.vben-pop-leave-to {
  opacity: 0;
  transform: translateY(8px);
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(8px); }
  to { opacity: 1; transform: translateY(0); }
}

textarea {
  scrollbar-width: none;
}



/* 针对 AI 回复内容的 Markdown 深度定制 */
.markdown-body {
  word-break: break-word;
  line-height: 1.75;
  color: #d1d5db; /* 稍微柔和一点的白色 */
}

/* 清除最后一个元素的下边距，防止气泡底部留白过大 */
.markdown-body :deep(> *:last-child) {
  margin-bottom: 0 !important;
}

/* 标题样式优化：增加层次感 */
.markdown-body :deep(h1), 
.markdown-body :deep(h2), 
.markdown-body :deep(h3) {
  color: #ffffff;
  font-weight: 600;
  margin-top: 1.5rem;
  margin-bottom: 0.75rem;
  line-height: 1.3;
}
.markdown-body :deep(h1) { font-size: 1.5rem; border-bottom: 1px solid #30363d; padding-bottom: 0.3rem; }
.markdown-body :deep(h2) { font-size: 1.25rem; }
.markdown-body :deep(h3) { font-size: 1.1rem; }

/* 段落优化 */
.markdown-body :deep(p) {
  margin-bottom: 1rem;
}

/* 列表样式优化：解决嵌套缩进 */
.markdown-body :deep(ul), 
.markdown-body :deep(ol) {
  padding-left: 1.5rem;
  margin-bottom: 1rem;
}
.markdown-body :deep(li) {
  margin-bottom: 0.25rem;
}
.markdown-body :deep(li > ul), 
.markdown-body :deep(li > ol) {
  margin-top: 0.25rem;
  margin-bottom: 0;
}

/* 引用块 (Blockquote) */
.markdown-body :deep(blockquote) {
  margin: 1rem 0;
  padding: 0 1rem;
  color: #8b949e;
  border-left: 0.25rem solid #0960bd;
  background: rgba(9, 96, 189, 0.05);
}

/* 行内代码：更明显的区分 */
.markdown-body :deep(code:not(pre code)) {
  font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace;
  background-color: rgba(110, 118, 129, 0.2);
  padding: 0.2rem 0.4rem;
  border-radius: 6px;
  font-size: 88%;
  color: #e6edf3;
}

/* 代码块：增加滚动条美化和溢出处理 */
.markdown-body :deep(pre) {
  background-color: #0d1117; /* 更深邃的背景 */
  padding: 1rem;
  border-radius: 12px;
  overflow-x: auto;
  margin: 1.2rem 0;
  border: 1px solid #30363d;
}

.markdown-body :deep(pre code) {
  background-color: transparent;
  padding: 0;
  font-size: 13px;
  line-height: 1.5;
  color: #e6edf3;
  font-family: 'Fira Code', 'Cascadia Code', monospace;
}

/* 表格样式：AI 经常返回表格，这部分很重要 */
.markdown-body :deep(table) {
  width: 100%;
  border-collapse: collapse;
  margin-bottom: 1rem;
  font-size: 13px;
}
.markdown-body :deep(th), 
.markdown-body :deep(td) {
  padding: 8px 12px;
  border: 1px solid #30363d;
}
.markdown-body :deep(th) {
  background-color: rgba(110, 118, 129, 0.1);
  text-align: left;
}

/* 链接动画 */
.markdown-body :deep(a) {
  color: #4493f8;
  text-decoration: none;
  border-bottom: 1px solid transparent;
  transition: border-color 0.2s;
}
.markdown-body :deep(a:hover) {
  border-bottom-color: #4493f8;
}

/* 优化代码块内的滚动条 */
.markdown-body :deep(pre)::-webkit-scrollbar {
  height: 6px;
}
.markdown-body :deep(pre)::-webkit-scrollbar-thumb {
  background: #30363d;
  border-radius: 10px;
}
.markdown-body :deep(pre)::-webkit-scrollbar-thumb:hover {
  background: #484f58;
}
</style>
