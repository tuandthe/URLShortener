import React, { useState, useEffect } from 'react';
import { urlHistoryService, UrlHistoryItem } from '../services/urlHistoryService';
import './HistoryList.css';

interface HistoryListProps {
    onUrlSelect?: (item: UrlHistoryItem) => void;
    limit?: number; // Số lượng item hiển thị (mặc định: tất cả)
    onViewAll?: () => void; // Callback khi click "Xem tất cả"
}

const HistoryList: React.FC<HistoryListProps> = ({ onUrlSelect, limit, onViewAll }) => {
    const [history, setHistory] = useState<UrlHistoryItem[]>([]);
    const [copiedId, setCopiedId] = useState<string | null>(null);

    // Load lịch sử khi component mount
    useEffect(() => {
        loadHistory();
    }, []);

    const loadHistory = () => {
        const items = urlHistoryService.getHistory();
        setHistory(items);
    };

    const handleCopy = async (item: UrlHistoryItem) => {
        try {
            await navigator.clipboard.writeText(item.shortUrl);
            setCopiedId(item.id);
            setTimeout(() => setCopiedId(null), 2000);
        } catch (error) {
            console.error('Failed to copy:', error);
            alert('Không thể sao chép URL');
        }
    };

    const handleDelete = (id: string) => {
        if (window.confirm('Bạn có chắc muốn xóa URL này khỏi lịch sử?')) {
            urlHistoryService.removeFromHistory(id);
            loadHistory();
        }
    };
    const formatDate = (dateString: string) => {
        const date = new Date(dateString);
        const now = new Date();
        const diffMs = now.getTime() - date.getTime();
        const diffMins = Math.floor(diffMs / 60000);
        const diffHours = Math.floor(diffMs / 3600000);
        const diffDays = Math.floor(diffMs / 86400000);

        if (diffMins < 1) return 'Vừa xong';
        if (diffMins < 60) return `${diffMins} phút trước`;
        if (diffHours < 24) return `${diffHours} giờ trước`;
        if (diffDays < 7) return `${diffDays} ngày trước`;

        return date.toLocaleDateString('vi-VN', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
        });
    };

    const truncateUrl = (url: string, maxLength: number = 50) => {
        if (url.length <= maxLength) return url;
        return url.substring(0, maxLength) + '...';
    };

    if (history.length === 0) {
        return (
            <div className="history-empty">
                <p>📋 Chưa có lịch sử rút gọn URL</p>
            </div>
        );
    }

    // ✅ Áp dụng limit nếu có
    const displayedHistory = limit ? history.slice(0, limit) : history;
    const hasMore = limit && history.length > limit;

    return (
        <div className="history-container">
            <div className="history-header">
                <h3>📜 Lịch sử rút gọn ({history.length})</h3>

            </div>

            <div className="history-list">
                {displayedHistory.map((item) => (
                    <div key={item.id} className="history-item">
                        <div className="history-item-content">
                            <div className="history-item-urls">
                                <div className="original-url" title={item.originalUrl}>
                                    <span className="url-label">Gốc:</span>
                                    <span className="url-text">{truncateUrl(item.originalUrl)}</span>
                                </div>
                                <div className="short-url">
                                    <span className="url-label">Rút gọn:</span>
                                    <a
                                        href={item.shortUrl}
                                        target="_blank"
                                        rel="noopener noreferrer"
                                        className="url-link"
                                    >
                                        {item.shortUrl}
                                    </a>
                                </div>
                            </div>
                            <div className="history-item-meta">
                                <span className="created-time">{formatDate(item.createdAt)}</span>
                            </div>
                        </div>

                        <div className="history-item-actions">
                            <button
                                className={`copy-btn ${copiedId === item.id ? 'copied' : ''}`}
                                onClick={() => handleCopy(item)}
                                title="Sao chép URL rút gọn"
                            >
                                {copiedId === item.id ? '✓ Đã copy' : '📋 Sao chép'}
                            </button>
                            <button
                                className="delete-btn"
                                onClick={() => handleDelete(item.id)}
                                title="Xóa khỏi lịch sử"
                            >
                                🗑️
                            </button>
                        </div>
                    </div>
                ))}
            </div>

            {/* ✅ Nút "Xem tất cả" - Navigate đến trang full history */}
            {hasMore && onViewAll && (
                <div className="show-more-container">
                    <button
                        className="show-more-btn"
                        onClick={onViewAll}
                    >
                        📋 Xem tất cả ({history.length} URL)
                    </button>
                </div>
            )}
        </div>
    );
};

export default HistoryList;
