import { ref, computed } from 'vue'

interface PaginationOptions {
  totalItems: number
  itemsPerPage: number
  currentPage: number
}

export function usePagination(options: PaginationOptions) {
  const totalItems = ref(options.totalItems)
  const itemsPerPage = ref(options.itemsPerPage)
  const currentPage = ref(options.currentPage)

  const totalPages = computed(() => {
    return Math.ceil(totalItems.value / itemsPerPage.value)
  })

  const paginatedData = computed(() => {
    const start = (currentPage.value - 1) * itemsPerPage.value
    const end = start + itemsPerPage.value
    return Array.from({ length: totalItems.value }, (_, i) => i + 1).slice(start, end)
  })

  const nextPage = () => {
    if (currentPage.value < totalPages.value) {
      currentPage.value++
    }
  }

  const prevPage = () => {
    if (currentPage.value > 1) {
      currentPage.value--
    }
  }

  const goToPage = (page: number) => {
    if (page >= 1 && page <= totalPages.value) {
      currentPage.value = page
    }
  }

  return {
    currentPage,
    totalPages,
    paginatedData,
    nextPage,
    prevPage,
    goToPage
  }
}