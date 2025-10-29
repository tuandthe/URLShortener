/**
 * Service quản lý lịch sử URL rút gọn trong LocalStorage
 */

export interface UrlHistoryItem {
    id: string;
    originalUrl: string;
    shortUrl: string;
    shortCode: string;
    createdAt: string;
}

const STORAGE_KEY = 'url_shortener_history';
const MAX_HISTORY_ITEMS = 50; // Giới hạn số lượng để tránh đầy storage

class UrlHistoryService {
    /**
     * Lấy toàn bộ lịch sử từ LocalStorage
     */
    getHistory(): UrlHistoryItem[] {
        try {
            const data = localStorage.getItem(STORAGE_KEY);
            if (!data) return [];

            const history = JSON.parse(data) as UrlHistoryItem[];
            // Sắp xếp theo thời gian mới nhất
            return history.sort((a, b) =>
                new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
            );
        } catch (error) {
            console.error('Error reading history from localStorage:', error);
            return [];
        }
    }

    /**
     * Thêm URL mới vào lịch sử
     */
    addToHistory(item: UrlHistoryItem): void {
        try {
            let history = this.getHistory();

            // Kiểm tra xem URL đã tồn tại chưa (dựa vào shortCode)
            const existingIndex = history.findIndex(h => h.shortCode === item.shortCode);

            if (existingIndex !== -1) {
                // Nếu đã tồn tại, xóa item cũ để thêm lên đầu
                history.splice(existingIndex, 1);
            }

            // Thêm item mới vào đầu mảng
            history.unshift(item);

            // Giới hạn số lượng items
            if (history.length > MAX_HISTORY_ITEMS) {
                history = history.slice(0, MAX_HISTORY_ITEMS);
            }

            localStorage.setItem(STORAGE_KEY, JSON.stringify(history));
        } catch (error) {
            console.error('Error adding to history:', error);
        }
    }

    /**
     * Xóa một item khỏi lịch sử
     */
    removeFromHistory(id: string): void {
        try {
            const history = this.getHistory();
            const filteredHistory = history.filter(item => item.id !== id);
            localStorage.setItem(STORAGE_KEY, JSON.stringify(filteredHistory));
        } catch (error) {
            console.error('Error removing from history:', error);
        }
    }

    /**
     * Xóa toàn bộ lịch sử
     */
    clearHistory(): void {
        try {
            localStorage.removeItem(STORAGE_KEY);
        } catch (error) {
            console.error('Error clearing history:', error);
        }
    }

    /**
     * Lấy số lượng items trong lịch sử
     */
    getHistoryCount(): number {
        return this.getHistory().length;
    }
}

export const urlHistoryService = new UrlHistoryService();
