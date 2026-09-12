import { useEffect, useMemo, useState } from 'react'
import './App.css'

type SpeakerSummary = {
  id: number
  name: string
  title: string
  company: string
}

type SpeakerDetails = SpeakerSummary & {
  bio: string
  photoUrl: string
}

type Session = {
  id: number
  title: string
  summary: string
  description: string
  track: string
  level: string
  room: string
  startTimeUtc: string
  endTimeUtc: string
  speaker: SpeakerSummary
}

function App() {
  const [sessions, setSessions] = useState<Session[]>([])
  const [speakers, setSpeakers] = useState<SpeakerDetails[]>([])
  const [favoriteSessionIds, setFavoriteSessionIds] = useState<number[]>([])
  const [selectedSessionId, setSelectedSessionId] = useState<number | null>(null)
  const [showFavoritesOnly, setShowFavoritesOnly] = useState(false)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [sessionsResponse, speakersResponse, favoritesResponse] = await Promise.all([
          fetch('/api/sessions'),
          fetch('/api/speakers'),
          fetch('/api/favorites'),
        ])

        if (!sessionsResponse.ok || !speakersResponse.ok || !favoritesResponse.ok) {
          throw new Error('Unable to load conference data')
        }

        const sessionsData = (await sessionsResponse.json()) as Session[]
        const speakersData = (await speakersResponse.json()) as SpeakerDetails[]
        const favoritesData = (await favoritesResponse.json()) as Session[]

        setSessions(sessionsData)
        setSpeakers(speakersData)
        setFavoriteSessionIds(favoritesData.map((session) => session.id))
        setSelectedSessionId(sessionsData[0]?.id ?? null)
      } catch (error) {
        console.error(error)
      } finally {
        setLoading(false)
      }
    }

    void fetchData()
  }, [])

  const selectedSession = useMemo(
    () => sessions.find((session) => session.id === selectedSessionId) ?? sessions[0],
    [selectedSessionId, sessions],
  )

  const selectedSpeaker = useMemo(
    () => speakers.find((speaker) => speaker.name === selectedSession?.speaker.name) ?? speakers[0],
    [selectedSession, speakers],
  )

  const visibleSessions = useMemo(
    () => (showFavoritesOnly ? sessions.filter((session) => favoriteSessionIds.includes(session.id)) : sessions),
    [favoriteSessionIds, sessions, showFavoritesOnly],
  )

  const toggleFavorite = async (sessionId: number) => {
    const isFavorited = favoriteSessionIds.includes(sessionId)

    try {
      if (isFavorited) {
        const response = await fetch(`/api/favorites/${sessionId}`, { method: 'DELETE' })
        if (!response.ok) {
          throw new Error('Unable to remove favorite')
        }

        setFavoriteSessionIds((current) => current.filter((id) => id !== sessionId))
        return
      }

      const response = await fetch('/api/favorites', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ sessionId }),
      })

      if (!response.ok) {
        throw new Error('Unable to add favorite')
      }

      setFavoriteSessionIds((current) => [...current, sessionId])
    } catch (error) {
      console.error(error)
    }
  }

  return (
    <div className="app-shell">
      <header className="topbar">
        <div>
          <p className="eyebrow">Conference planner</p>
          <h1>SessionHub</h1>
        </div>
        <div className="header-actions">
          <button
            type="button"
            className={`toggle-favorites ${showFavoritesOnly ? 'active' : ''}`}
            onClick={() => setShowFavoritesOnly((value) => !value)}
          >
            {showFavoritesOnly ? 'Show all' : 'Favorites'} ({favoriteSessionIds.length})
          </button>
          <div className="header-pill">15 sessions • 10 speakers</div>
        </div>
      </header>

      <main className="layout">
        <aside className="panel session-list-panel">
          <div className="panel-header">
            <h2>{showFavoritesOnly ? 'Favorite sessions' : 'Sessions'}</h2>
            <span>{visibleSessions.length}</span>
          </div>

          {loading ? (
            <p className="empty-state">Loading sessions…</p>
          ) : visibleSessions.length === 0 ? (
            <p className="empty-state">{showFavoritesOnly ? 'No favorites yet.' : 'No sessions available right now.'}</p>
          ) : (
            <ul className="session-list">
              {visibleSessions.map((session) => {
                const isFavorited = favoriteSessionIds.includes(session.id)

                return (
                  <li key={session.id}>
                    <div
                      className={`session-item ${selectedSession?.id === session.id ? 'selected' : ''}`}
                      onClick={() => setSelectedSessionId(session.id)}
                      onKeyDown={(event) => {
                        if (event.key === 'Enter' || event.key === ' ') {
                          event.preventDefault()
                          setSelectedSessionId(session.id)
                        }
                      }}
                      role="button"
                      tabIndex={0}
                    >
                      <div className="session-meta-row">
                        <span className="badge track">{session.track}</span>
                        <span className="badge level">{session.level}</span>
                      </div>
                      <div className="session-title-row">
                        <h3>{session.title}</h3>
                        <button
                          type="button"
                          className={`favorite-button ${isFavorited ? 'favorited' : ''}`}
                          onClick={(event) => {
                            event.stopPropagation()
                            void toggleFavorite(session.id)
                          }}
                          aria-label={isFavorited ? 'Remove favorite' : 'Add favorite'}
                          title={isFavorited ? 'Remove favorite' : 'Add favorite'}
                        >
                          {isFavorited ? '★' : '☆'}
                        </button>
                      </div>
                      <p>{session.speaker.name}</p>
                      <div className="session-meta-row small-row">
                        <span>{new Date(session.startTimeUtc).toLocaleString([], { month: 'short', day: 'numeric', hour: 'numeric', minute: '2-digit' })}</span>
                        <span>{session.room}</span>
                      </div>
                    </div>
                  </li>
                )
              })}
            </ul>
          )}
        </aside>

        <section className="panel session-detail-panel">
          {selectedSession ? (
            <>
              <div className="detail-header">
                <div>
                  <p className="eyebrow">{selectedSession.track}</p>
                  <h2>{selectedSession.title}</h2>
                </div>
                <div className="detail-tags">
                  <button
                    type="button"
                    className={`favorite-toggle ${favoriteSessionIds.includes(selectedSession.id) ? 'favorited' : ''}`}
                    onClick={() => void toggleFavorite(selectedSession.id)}
                  >
                    {favoriteSessionIds.includes(selectedSession.id) ? '★ Remove favorite' : '☆ Favorite'}
                  </button>
                  <span className="badge dark">{selectedSession.level}</span>
                  <span className="badge dark">{selectedSession.room}</span>
                </div>
              </div>

              <p className="summary">{selectedSession.summary}</p>

              <div className="meta-grid">
                <div>
                  <label>Speaker</label>
                  <strong>{selectedSession.speaker.name}</strong>
                </div>
                <div>
                  <label>Time</label>
                  <strong>
                    {new Date(selectedSession.startTimeUtc).toLocaleString([], {
                      month: 'short',
                      day: 'numeric',
                      hour: 'numeric',
                      minute: '2-digit',
                    })}{' '}
                    -{' '}
                    {new Date(selectedSession.endTimeUtc).toLocaleString([], {
                      month: 'short',
                      day: 'numeric',
                      hour: 'numeric',
                      minute: '2-digit',
                    })}
                  </strong>
                </div>
                <div>
                  <label>Track</label>
                  <strong>{selectedSession.track}</strong>
                </div>
              </div>

              <div className="description-block">
                <h3>About this session</h3>
                <p>{selectedSession.description}</p>
              </div>

              <div className="speaker-card">
                <div className="speaker-header">
                  <div>
                    <p className="eyebrow">Featured speaker</p>
                    <h3>{selectedSpeaker?.name ?? selectedSession.speaker.name}</h3>
                    <p className="speaker-role">{selectedSpeaker?.title ?? selectedSession.speaker.title}</p>
                    <p className="speaker-company">{selectedSpeaker?.company ?? selectedSession.speaker.company}</p>
                  </div>
                </div>
                <p className="speaker-bio">{selectedSpeaker?.bio ?? 'Speaker bio is available in the API response.'}</p>
              </div>
            </>
          ) : (
            <p className="empty-state">Select a session to view details.</p>
          )}
        </section>
      </main>
    </div>
  )
}

export default App
